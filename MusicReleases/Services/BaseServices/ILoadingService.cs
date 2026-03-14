using JakubKastner.MusicReleases.BackgroundTasks.Enums;

namespace JakubKastner.MusicReleases.Services.BaseServices
{
	public interface ILoadingService
	{
		string ActiveClass { get; }
		bool Loading { get; }

		event Action? LoadingStateChanged;

		void Dispose();
		bool IsLoading(BackgroundTaskType type);
	}
}