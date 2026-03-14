using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.Enums;
using System.Globalization;

namespace JakubKastner.MusicReleases.BackgroundTasks.Extensions;

internal static class BackgroundTaskExtensions
{
	// STEPS

	public static async Task RunStep(this BackgroundTask task, string name, BackgroundTaskCategory category, Func<CancellationToken, Task> work)
	{
		await RunStepInternal(task, name, category, work, task.Ct);
	}

	public static Task<T> RunStep<T>(this BackgroundTask task, string name, BackgroundTaskCategory category, Func<CancellationToken, Task<T>> body)
	{
		return RunStepInternal(task, name, category, body, task.Ct);
	}

	private static async Task RunStepInternal(this BackgroundTask task, string name, BackgroundTaskCategory category, Func<CancellationToken, Task> work, CancellationToken ct)
	{
		await using (await task.BeginStep(name, category, ct))
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

	private static async Task<T> RunStepInternal<T>(this BackgroundTask task, string name, BackgroundTaskCategory category, Func<CancellationToken, Task<T>> body, CancellationToken ct)
	{
		await using (await task.BeginStep(name, category, ct))
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

	private static async ValueTask<IAsyncDisposable> BeginStep(this BackgroundTask task, string name, BackgroundTaskCategory category, CancellationToken ct)
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

	// SEGMENTS

	public static async Task<T> RunSegment<T>(this BackgroundTask task, string label, Func<CancellationToken, Task<T>> body)
	{
		await using var seg = await task.BeginSegment(label);
		return await body(task.Ct);
	}

	public static async Task RunSegment(this BackgroundTask task, string label, Func<CancellationToken, Task> body)
	{
		await using var seg = await task.BeginSegment(label);
		await body(task.Ct);
	}

	private static ValueTask<IBackgroundTaskSubProgressScope> BeginSegment(this BackgroundTask task, string label)
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

	private static async ValueTask<IBackgroundTaskSubProgressScope> BeginSubSegmentAsync(this BackgroundTask task, double from, double to, string segmentLabel, bool writeStartMeta = false)
	{
		// sub segment

		if (task.Steps.Count == 0)
		{
			return new BackgroundTaskSubProgressScopeNoop();
		}

		var idx = task.CurrentStepIndex;
		if (idx < 0 || idx >= task.Steps.Count)
		{
			return new BackgroundTaskSubProgressScopeNoop();
		}

		var step = task.Steps[idx];

		from = Math.Clamp(from, 0, 1);
		to = Math.Clamp(to, 0, 1);
		if (to < from) (from, to) = (to, from);

		if (writeStartMeta)
		{
			var seq = (step.SubprocessSequence + 1).ToString("D2", CultureInfo.InvariantCulture);
			step.Meta[$"seg.{seq}.label"] = segmentLabel;
			step.Meta[$"seg.{seq}.from"] = from.ToString("F3", CultureInfo.InvariantCulture);
			step.Meta[$"seg.{seq}.to"] = to.ToString("F3", CultureInfo.InvariantCulture);
		}

		task.SetSubProgress(from, segmentLabel + ":begin");

		var scope = new BackgroundTaskSubProgressScope(task, step, from, to, segmentLabel);
		return scope;
	}

	public static void BeginAutoSegments(this BackgroundTask task, int count)
	{
		var step = task.Steps[task.CurrentStepIndex];
		step.Meta["seg.count"] = count.ToString();
		step.Meta["seg.index"] = "0";
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
		step.SubprocessSequence++;

		if (label.IsNotNullOrEmpty())
		{
			var seq = step.SubprocessSequence.ToString("D2", CultureInfo.InvariantCulture);
			step.Meta[$"sub.{seq}.label"] = label;
			step.Meta[$"sub.{seq}.ms"] = delta.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);
		}
		else
		{
			var seq = step.SubprocessSequence.ToString("D2", CultureInfo.InvariantCulture);
			step.Meta[$"sub.{seq}.ms"] = delta.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);
		}

		task.RecalculateProgress();
		task.NotifyChange();
	}
}