using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.BackgroundTasks;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public static class BackgroundTaskStepExtensions
{
	public static void MarkFailed(this BackgroundTaskStep step, Exception ex, bool transient = false, string? code = null)
	{
		if (step.Ended)
		{
			return;
		}

		step.Status = BackgroundTaskStatus.Failed;
		step.FinishedAt = DateTimeOffset.UtcNow;
		step.ErrorMessage = ex.Message;
		step.ErrorCode = code ?? "ERR_STEP";
		step.Transient = transient;
	}

	public static void MarkCanceled(this BackgroundTaskStep step)
	{
		if (step.Ended)
		{
			return;
		}

		step.Status = BackgroundTaskStatus.Canceled;
		step.FinishedAt = DateTimeOffset.UtcNow;
	}

	public static void MarkFinished(this BackgroundTaskStep step)
	{
		if (step.Ended)
		{
			return;
		}

		step.SubProgress = 1.0;
		step.Status = BackgroundTaskStatus.Finished;
		step.FinishedAt = DateTimeOffset.UtcNow;
	}
}