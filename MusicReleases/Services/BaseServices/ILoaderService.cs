using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ILoaderService
{
	bool Loading { get; }
	string LoadingClass { get; }

	event Action? LoadingStateChanged;

	bool IsLoading(LoadingType type);
	bool IsLoading(LoadingType type, LoadingCategory category);
	void StartLoading(LoadingType type, LoadingCategory category);
	void StopLoading(LoadingType type, LoadingCategory category);
}