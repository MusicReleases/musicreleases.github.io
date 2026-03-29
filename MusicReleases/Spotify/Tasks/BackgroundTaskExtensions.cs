using System.Collections.Concurrent;
using System.Globalization;

namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal static class BackgroundTaskExtensions
{
	private static readonly ConcurrentDictionary<Guid, string> _stepLabels = new();

	// STEPS

	public static async Task RunStep(this BackgroundTask2 task, string name, BackgroundTaskCategory category, Func<CancellationToken, Task> work)
	{
		await RunStepInternal(task, name, category, work, task.Ct);
	}
	public static async Task RunStep(this BackgroundTask2 task, Guid stepId, string name, BackgroundTaskCategory category, Func<CancellationToken, Task> work)
	{
		await RunStepInternal(task, stepId, name, category, work, task.Ct);
	}

	public static Task<T> RunStep<T>(this BackgroundTask2 task, string name, BackgroundTaskCategory category, Func<CancellationToken, Task<T>> body)
	{
		return RunStepInternal(task, name, category, body, task.Ct);
	}

	public static Task<T> RunStep<T>(this BackgroundTask2 task, Guid stepId, string name, BackgroundTaskCategory category, Func<CancellationToken, Task<T>> body)
	{
		return RunStepInternal(task, stepId, name, category, body, task.Ct);
	}

	private static async Task RunStepInternal(this BackgroundTask2 task, string name, BackgroundTaskCategory category, Func<CancellationToken, Task> work, CancellationToken ct)
	{
		await using (await task.BeginStep(name, category, ct))
		{
			try
			{
				await work(ct);

				var step = task.Steps[task.CurrentStepIndex];
				step.MarkFinished();
				task.StepSink.MarkStepCompleted(step.StepId, true);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex)
			{
				var step = task.Steps[task.CurrentStepIndex];
				step.MarkFailed(ex);
				task.StepSink.MarkStepCompleted(step.StepId, false);
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}

	private static async Task RunStepInternal(this BackgroundTask2 task, Guid stepId, string name, BackgroundTaskCategory category, Func<CancellationToken, Task> work, CancellationToken ct)
	{
		await using (await task.BeginStep(stepId, name, category, ct))
		{
			try
			{
				await work(ct);

				var step = task.Steps[task.CurrentStepIndex];
				step.MarkFinished();
				task.StepSink.MarkStepCompleted(step.StepId, true);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception ex)
			{
				var step = task.Steps[task.CurrentStepIndex];
				step.MarkFailed(ex);
				task.StepSink.MarkStepCompleted(step.StepId, false);
				Console.WriteLine(ex);
				throw;
			}
		}
	}

	private static async Task<T> RunStepInternal<T>(this BackgroundTask2 task, string name, BackgroundTaskCategory category, Func<CancellationToken, Task<T>> body, CancellationToken ct)
	{
		await using (await task.BeginStep(name, category, ct))
		{
			try
			{
				var result = await body(ct);

				var step = task.Steps[task.CurrentStepIndex];
				step.MarkFinished();
				task.StepSink.MarkStepCompleted(step.StepId, true);

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
				task.StepSink.MarkStepCompleted(step.StepId, false);
				Console.WriteLine(ex.ToString());
				throw;
			}
		}
	}

	private static async Task<T> RunStepInternal<T>(this BackgroundTask2 task, Guid stepId, string name, BackgroundTaskCategory category, Func<CancellationToken, Task<T>> body, CancellationToken ct)
	{
		await using (await task.BeginStep(stepId, name, category, ct))
		{
			try
			{
				var result = await body(ct);

				var step = task.Steps[task.CurrentStepIndex];
				step.MarkFinished();
				task.StepSink.MarkStepCompleted(step.StepId, true);

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
				task.StepSink.MarkStepCompleted(step.StepId, false);
				Console.WriteLine(ex);
				throw;
			}
		}
	}

	private static async ValueTask<IAsyncDisposable> BeginStep(this BackgroundTask2 task, string name, BackgroundTaskCategory category, CancellationToken ct)
	{
		var step = new BackgroundTaskStep2(name, category);
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

	private static async ValueTask<IAsyncDisposable> BeginStep(this BackgroundTask2 task, Guid stepId, string name, BackgroundTaskCategory category, CancellationToken ct)
	{
		var step = new BackgroundTaskStep2(name, category)
		{
			StepId = stepId
		};
		_stepLabels.TryAdd(step.StepId, step.Name);
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

	public static async Task WaitForStep(this BackgroundTask2 task, BackgroundTaskStep2 step, Guid dependsOnStepId)
	{
		step.IsWaiting = true;
		step.WaitingForStepId = dependsOnStepId;
		step.NotifyChange();

		try
		{
			var ok = await task.WaitForStep(dependsOnStepId);

			if (!ok)
			{
				throw new InvalidOperationException($"Dependency step {dependsOnStepId} failed or was canceled.");
			}
		}
		finally
		{
			step.IsWaiting = false;
			step.WaitingForStepId = null;
			step.NotifyChange();
		}
	}

	// SEGMENTS

	public static async Task<T> RunSegment<T>(this BackgroundTask2 task, string label, Func<CancellationToken, Task<T>> body)
	{
		await using var seg = await task.BeginSegment(label);
		return await body(task.Ct);
	}

	public static async Task RunSegment(this BackgroundTask2 task, string label, Func<CancellationToken, Task> body)
	{
		await using var seg = await task.BeginSegment(label);
		await body(task.Ct);
	}

	private static ValueTask<IBackgroundTaskSubProgressScope> BeginSegment(this BackgroundTask2 task, string label)
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

	private static async ValueTask<IBackgroundTaskSubProgressScope> BeginSubSegmentAsync(this BackgroundTask2 task, double from, double to, string segmentLabel, bool writeStartMeta = false)
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

		var scope = new BackgroundTaskSubProgressScope2(task, step, from, to, segmentLabel);
		return scope;
	}

	public static void BeginAutoSegments(this BackgroundTask2 task, int count)
	{
		var step = task.Steps[task.CurrentStepIndex];
		step.Meta["seg.count"] = count.ToString();
		step.Meta["seg.index"] = "0";
	}

	public static void SetSubProgress(this BackgroundTask2 task, double value, string? label = null)
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

	public static string? GetStepLabel(Guid stepId)
	{
		return _stepLabels.TryGetValue(stepId, out var label) ? label : null;
	}
}