using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ILoaderService
{
	bool Loading { get; }
	string ActiveClass { get; }

	event Action? LoadingStateChanged;

	bool IsLoading(BackgroundTaskType type);
	bool IsLoading(BackgroundTaskType type, BackgroundTaskCategory category);
	void StartLoading(BackgroundTaskType type, BackgroundTaskCategory category);
	void StopLoading(BackgroundTaskType type, BackgroundTaskCategory category);
}