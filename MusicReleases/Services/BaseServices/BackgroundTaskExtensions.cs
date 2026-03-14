using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.BackgroundTasks;
using System.Globalization;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public static class BackgroundTaskExtensions
{
	public static async Task Step(this BackgroundTask task, string name, BackgroundTaskCategory category, Func<CancellationToken, Task> work)
	{
		await RunStepAsyncInternal(task, name, category, task.Token, work);
	}

	private static async Task RunStepAsyncInternal(this BackgroundTask task, string name, BackgroundTaskCategory category, CancellationToken ct, Func<CancellationToken, Task> work)
	{
		await using (await task.BeginStepAsync(name, category, ct))
		{
			try
			{
				await work(ct);

				var step = task.Steps[task.CurrentStepIndex];
				step.MarkFinished();
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex)
			{
				var step = task.Steps[task.CurrentStepIndex];
				step.MarkFailed(ex);
				throw;
			}
		}
	}
	private static async ValueTask<IAsyncDisposable> BeginStepAsync(this BackgroundTask task, string name, BackgroundTaskCategory category, CancellationToken ct = default)
	{
		var step = new BackgroundTaskStep(name, category);
		task.AddStep(step);

		CancellationTokenRegistration? ctr = null;
		if (ct.CanBeCanceled)
		{
			ctr = ct.Register(() =>
			{
				task.IsCancelRequested = true;
				step.NotifyChange();
			});
		}

		return new BackgroundTaskStepScope(task, step, ct, ctr);
	}

	public static void BeginAutoSegments(this BackgroundTask task, int count)
	{
		var step = task.Steps[task.CurrentStepIndex];
		step.Meta["seg.count"] = count.ToString();
		step.Meta["seg.index"] = "0";
	}

	public static ValueTask<IBackgroundTaskSubProgressScope> Segment(this BackgroundTask task, string label)
	{
		var idx = task.CurrentStepIndex;
		var step = task.Steps[idx];

		var count = int.Parse(step.Meta["seg.count"]);
		var index = int.Parse(step.Meta["seg.index"]);


		// auto bounds
		var segmentSize = 1.0 / count;
		var from = index * segmentSize;
		var to = from + segmentSize;

		// move index
		step.Meta["seg.index"] = (index + 1).ToString();
		return task.BeginSubSegmentAsync(from, to, label);
	}


	public static Task<T> StepAsync<T>(this BackgroundTask task, string name, BackgroundTaskCategory category, Func<CancellationToken, Task<T>> body)
	{
		return RunStepAsyncInternal(task, name, category, task.Token, body);
	}

	private static async Task<T> RunStepAsyncInternal<T>(this BackgroundTask task, string name, BackgroundTaskCategory category, CancellationToken ct, Func<CancellationToken, Task<T>> body)
	{
		await using (await task.BeginStepAsync(name, category, ct))
		{
			try
			{
				var result = await body(ct);

				var step = task.Steps[task.CurrentStepIndex];
				step.MarkFinished();

				return result;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex)
			{
				var step = task.Steps[task.CurrentStepIndex];
				step.MarkFailed(ex);
				throw;
			}
		}
	}


	public static void SetSubProgress(this BackgroundTask task, double value, string? label = null)
	{
		if (task.Steps.Count == 0)
		{
			return;
		}

		var idx = task.CurrentStepIndex;
		if (idx < 0 || idx >= task.Steps.Count)
		{
			return;
		}

		var step = task.Steps[idx];
		var now = DateTimeOffset.UtcNow;
		var last = step.LastSubProgressAt ?? step.StartedAt;
		var delta = now - last;

		step.SubProgress = Math.Clamp(value, 0, 1);
		step.LastSubProgressAt = now;
		step.SubSeq++;

		if (label.IsNotNullOrEmpty())
		{
			var seq = step.SubSeq.ToString("D2", CultureInfo.InvariantCulture);
			step.Meta[$"sub.{seq}.label"] = label;
			step.Meta[$"sub.{seq}.ms"] = delta.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);
		}
		else
		{
			var seq = step.SubSeq.ToString("D2", CultureInfo.InvariantCulture);
			step.Meta[$"sub.{seq}.ms"] = delta.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);
		}

		task.RecalculateProgress();
		task.NotifyChange();
	}

	private static void AdvanceSubProgress(this BackgroundTask task, double delta, string? label = null)
	{
		if (task.Steps.Count == 0)
		{
			return;
		}

		var idx = task.CurrentStepIndex;
		if (idx < 0 || idx >= task.Steps.Count)
		{
			return;
		}

		var step = task.Steps[idx];
		var target = Math.Clamp(step.SubProgress + delta, 0, 1);
		task.SetSubProgress(target, label);
	}

	private static async ValueTask<IBackgroundTaskSubProgressScope> BeginSubSegmentAsync(this BackgroundTask task, double from, double to, string segmentLabel, bool writeStartMeta = false)
	{
		if (task.Steps.Count == 0)
		{
			return new BackgroudTaskNoopSubProgressScope();
		}

		var idx = task.CurrentStepIndex;
		if (idx < 0 || idx >= task.Steps.Count)
		{
			return new BackgroudTaskNoopSubProgressScope();
		}

		var step = task.Steps[idx];

		from = Math.Clamp(from, 0, 1);
		to = Math.Clamp(to, 0, 1);
		if (to < from) (from, to) = (to, from);

		if (writeStartMeta)
		{
			var seq = (step.SubSeq + 1).ToString("D2", CultureInfo.InvariantCulture);
			step.Meta[$"seg.{seq}.label"] = segmentLabel;
			step.Meta[$"seg.{seq}.from"] = from.ToString("F3", CultureInfo.InvariantCulture);
			step.Meta[$"seg.{seq}.to"] = to.ToString("F3", CultureInfo.InvariantCulture);
		}

		task.SetSubProgress(from, segmentLabel + ":begin");

		var scope = new BackgroundTaskSubProgressScope(task, step, from, to, segmentLabel);
		return scope;
	}


	public static T Require<T>(this BackgroundTask task, T value, string? message = null)
	{
		if (value is null)
		{
			throw new NullReferenceException(message ?? "Required value is null");
		}

		return value;
	}

	public static async Task<T> RequireAsync<T>(this BackgroundTask task, Task<T> asyncValue, string? message = null)
	{
		var result = await asyncValue;

		if (result is null)
		{
			throw new NullReferenceException(message ?? "Required value is null");
		}

		return result;
	}

	public static async Task<T?> AllowNull<T>(this BackgroundTask task, T value)
	{
		return value;
	}
	public static async Task<T?> AllowNullAsync<T>(this BackgroundTask task, Task<T?> asyncValue)
	{
		return await asyncValue;
	}



	public static async Task<T> SegmentAsync<T>(this BackgroundTask task, string label, Func<CancellationToken, Task<T>> body)
	{
		// začátek segmentu
		await using (var seg = await task.Segment(label))
		{
			return await body(task.Token);
		}
	}

	public static async Task SegmentAsync(this BackgroundTask task, string label, Func<CancellationToken, Task> body)
	{
		await using (var seg = await task.Segment(label))
		{
			await body(task.Token);
		}
	}

	public static void ReportLoopProgress(this BackgroundTask task, int index, int total, string? label = null)
	{
		double frac = total == 0 ? 1.0 : (double)index / total;
		task.SetSubProgress(frac, label);
	}
}