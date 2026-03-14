using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.BackgroundTasks.Extensions;

internal static class BackgroundTaskStepExtensions
{
	public static void MarkFailed(this BackgroundTaskStep step, Exception ex, string? code = null)
	{
		if (step.Ended)
		{
			return;
		}

		step.Status = BackgroundTaskStatus.Failed;
		step.FinishedAt = DateTimeOffset.UtcNow;
		step.ErrorMessage = ex.Message;
		step.ErrorCode = code ?? "ERR_STEP";
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