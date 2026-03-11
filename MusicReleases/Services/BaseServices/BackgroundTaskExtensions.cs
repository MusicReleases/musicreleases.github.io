using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.BackgroundTasks;
using System.Globalization;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public static class BackgroundTaskExtensions
{
	public static async ValueTask<IAsyncDisposable> BeginStepAsync(this BackgroundTask task, string name, BackgroundTaskCategory category, CancellationToken ct = default)
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

	public static void AdvanceSubProgress(this BackgroundTask task, double delta, string? label = null)
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

	public static async ValueTask<IBackgroundTaskSubProgressScope> BeginSubSegmentAsync(this BackgroundTask task, double from, double to, string segmentLabel, bool writeStartMeta = false)
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
	/*public static void AddMeta(this BackgroundTask task, string key, string value)
	{
		if (task.Steps.Count == 0) return;
		var step = task.Steps[task.CurrentStepIndex];
		step.Meta[key] = value;
		step.NotifyChange();
	}

	public static void AddMeta(this BackgroundTaskStep step, string key, string value)
	{
		step.Meta[key] = value;
		step.NotifyChange();
	}

	public static void AddLink(this BackgroundTaskStep step, string label, string url, string? rel = null)
	{
		step.Meta[$"link:{label}"] = url;
		step.NotifyChange();
	}*/

}
public static class BackgroundTaskStepExtensions
{
	public static void MarkFailed(this BackgroundTaskStep step, Exception ex, bool transient = false, string? code = null)
	{
		step.Status = BackgroundTaskStatus.Failed;
		step.FinishedAt = DateTimeOffset.UtcNow;
		step.ErrorMessage = ex.Message;
		step.ErrorCode = code ?? "ERR_STEP";
		step.Transient = transient;
	}

	public static void MarkCanceled(this BackgroundTaskStep step)
	{
		step.Status = BackgroundTaskStatus.Canceled;
		step.FinishedAt = DateTimeOffset.UtcNow;
	}

	public static void MarkFinished(this BackgroundTaskStep step)
	{
		step.Status = BackgroundTaskStatus.Finished;
		step.FinishedAt = DateTimeOffset.UtcNow;
	}
}