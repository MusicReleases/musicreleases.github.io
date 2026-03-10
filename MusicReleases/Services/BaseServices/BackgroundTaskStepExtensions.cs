using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;
using System.Globalization;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public sealed class BackgroundTaskMetadata : Dictionary<string, string>
{

}


public interface IBackgroundTaskStepScope : IAsyncDisposable
{
	BackgroundTaskStep Step { get; }
}

public static class BackgroundTaskStepExtensions
{
	public static async ValueTask<IBackgroundTaskStepScope> BeginStepAsync(this BackgroundTask task, string name, BackgroundTaskCategory cathegory)
	{
		var step = new BackgroundTaskStep
		{
			Name = name,
			Category = cathegory,
			Status = BackgroundTaskStatus.Running,
			Attempt = 1,
			StartedAt = DateTimeOffset.UtcNow
		};

		task.Steps.Add(step);
		task.CurrentStepIndex = task.Steps.Count - 1;

		task.Status = $"Running: {name}";
		task.Progress
			= task.Steps.Count == 0
			? 0
			: (double)task.Steps.Count(s => s.Status is BackgroundTaskStatus.Succeeded) / task.Steps.Count;

		task.NotifyChange();

		return new BackgroundTaskStepScope(task, step);
	}

	private sealed class BackgroundTaskStepScope : IBackgroundTaskStepScope
	{
		private readonly BackgroundTask _task;
		public BackgroundTaskStep Step { get; }

		public BackgroundTaskStepScope(BackgroundTask task, BackgroundTaskStep step)
		{
			_task = task;
			Step = step;
		}

		public ValueTask DisposeAsync()
		{
			if (Step.Status == BackgroundTaskStatus.Running)
			{
				Step.Status = BackgroundTaskStatus.Succeeded;
				Step.FinishedAt = DateTimeOffset.UtcNow;

				_task.Progress
					= _task.Steps.Count == 0
					? 1
					: (double)_task.Steps.Count(s => s.Status is BackgroundTaskStatus.Succeeded) / _task.Steps.Count;

				_task.NotifyChange();
			}
			return ValueTask.CompletedTask;
		}
	}

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

	public static void AddLink(this BackgroundTaskStep step, string label, string urlWeb, string urlApp, string? rel = null)
	{
		var newLink = new BackgroundTaskLink { Label = label, UrlWeb = urlWeb, UrlApp = urlApp, Rel = rel };
		step.Links.Add(newLink);
	}

	public static void AddLink(this BackgroundTask task, string label, string urlWeb, string urlApp, string? rel = null)
	{
		var newLink = new BackgroundTaskLink { Label = label, UrlWeb = urlWeb, UrlApp = urlApp, Rel = rel };
		task.Links.Add(newLink);
	}
}


public static class BackgroundTaskSubProcessExtensions
{
	/// <summary>
	/// Nastaví sub-progress v aktuálním kroku (0..1), zapíše čas od poslední změny,
	/// přidá meta položky (label, pořadí, delta ms) a přepočítá celkový Progress.
	/// </summary>
	public static void SetSubProgress(this BackgroundTask task, double value, string? label = null)
	{
		if (task.Steps.Count == 0) return;
		var idx = task.CurrentStepIndex;
		if (idx < 0 || idx >= task.Steps.Count) return;

		var step = task.Steps[idx];
		var now = DateTimeOffset.UtcNow;
		var last = step.LastSubProgressAt ?? step.StartedAt ?? now;
		var delta = now - last;

		step.SubProgress = Math.Clamp(value, 0, 1);
		step.LastSubProgressAt = now;
		step.SubSeq++;

		if (!string.IsNullOrWhiteSpace(label))
		{
			// Zapiš do Meta sekvenčně: sub.01.label, sub.01.ms, ...
			var seq = step.SubSeq.ToString("D2", CultureInfo.InvariantCulture);
			step.Meta[$"sub.{seq}.label"] = label;
			step.Meta[$"sub.{seq}.ms"] = delta.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);
		}
		else
		{
			// I bez labelu můžeme logovat hrubou delta ms
			var seq = step.SubSeq.ToString("D2", CultureInfo.InvariantCulture);
			step.Meta[$"sub.{seq}.ms"] = delta.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);
		}

		task.RecalculateProgress();
		task.NotifyChange();
	}

	/// <summary>
	/// Inkrementuje sub-progress o delta (kladné i záporné), se záznamem meta a delta-časem.
	/// </summary>
	public static void AdvanceSubProgress(this BackgroundTask task, double delta, string? label = null)
	{
		if (task.Steps.Count == 0) return;
		var idx = task.CurrentStepIndex;
		if (idx < 0 || idx >= task.Steps.Count) return;

		var step = task.Steps[idx];
		var target = Math.Clamp(step.SubProgress + delta, 0, 1);
		task.SetSubProgress(target, label);
	}
}


public interface IBackgroundTaskSubProgressScope : IAsyncDisposable
{
	/// <summary>Reportuj stav v rámci segmentu (0..1).</summary>
	void Report(double fraction, string? label = null);
}

public static class BackgroundTaskSubSegmentExtensions
{
	public static async ValueTask<IBackgroundTaskSubProgressScope> BeginSubSegmentAsync(
		this BackgroundTask task,
		double from,                            // např. 0.30
		double to,                              // např. 0.90
		string segmentLabel,                    // např. "DB:fetch+sort"
		bool writeStartMeta = false)
	{
		if (task.Steps.Count == 0)
			return new BackgroudTaskNoopSubProgressScope();

		var idx = task.CurrentStepIndex;
		if (idx < 0 || idx >= task.Steps.Count)
			return new BackgroudTaskNoopSubProgressScope();

		var step = task.Steps[idx];

		from = Math.Clamp(from, 0, 1);
		to = Math.Clamp(to, 0, 1);
		if (to < from) (from, to) = (to, from);

		var scope = new BackgroundTaskSubProgressScope(task, step, from, to, segmentLabel);
		if (writeStartMeta)
		{
			var seq = (step.SubSeq + 1).ToString("D2", CultureInfo.InvariantCulture);
			step.Meta[$"seg.{seq}.label"] = segmentLabel;
			step.Meta[$"seg.{seq}.from"] = from.ToString("F3", CultureInfo.InvariantCulture);
			step.Meta[$"seg.{seq}.to"] = to.ToString("F3", CultureInfo.InvariantCulture);
		}

		// Při startu segmentu můžeš rovnou nastavit na "from"
		task.SetSubProgress(from, segmentLabel + ":begin");

		return scope;
	}

	private sealed class BackgroundTaskSubProgressScope : IBackgroundTaskSubProgressScope
	{
		private readonly BackgroundTask _task;
		private readonly BackgroundTaskStep _step;
		private readonly double _from;
		private readonly double _to;
		private readonly string _segmentLabel;
		private readonly DateTimeOffset _startedAt = DateTimeOffset.UtcNow;
		private bool _disposed;

		public BackgroundTaskSubProgressScope(BackgroundTask task, BackgroundTaskStep step, double from, double to, string segmentLabel)
		{
			_task = task;
			_step = step;
			_from = from;
			_to = to;
			_segmentLabel = segmentLabel;
		}

		public void Report(double fraction, string? label = null)
		{
			if (_disposed) return;

			// Mapování 0..1 -> from..to
			fraction = Math.Clamp(fraction, 0, 1);
			var mapped = _from + ((_to - _from) * fraction);
			_task.SetSubProgress(mapped, label ?? $"{_segmentLabel}:{(int)(fraction * 100)}%");
		}

		public ValueTask DisposeAsync()
		{
			if (_disposed) return ValueTask.CompletedTask;
			_disposed = true;

			var endedAt = DateTimeOffset.UtcNow;
			var dur = endedAt - _startedAt;

			// Zapiš duration segmentu
			var seq = _step.SubSeq.ToString("D2", CultureInfo.InvariantCulture);
			_step.Meta[$"seg.{seq}.label"] = _segmentLabel;
			_step.Meta[$"seg.{seq}.duration.ms"] = dur.TotalMilliseconds.ToString("F0", CultureInfo.InvariantCulture);

			// Ujisti se, že na konci jsme na "to"
			_task.SetSubProgress(_to, _segmentLabel + ":end");

			return ValueTask.CompletedTask;
		}
	}

	private sealed class BackgroudTaskNoopSubProgressScope : IBackgroundTaskSubProgressScope
	{
		public void Report(double fraction, string? label = null) { }
		public ValueTask DisposeAsync() => ValueTask.CompletedTask;
	}
}
