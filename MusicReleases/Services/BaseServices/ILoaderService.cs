using JakubKastner.MusicReleases.Base;

namespace JakubKastner.MusicReleases.Services.BaseServices
{
	public interface ILoaderService
	{
		bool Loading { get; }
		string LoadingClass { get; }

		event Action? LoadingStateChanged;

		bool IsLoading(Enums.LoadingType type);
		bool IsLoading(Enums.LoadingType type, Enums.LoadingCategory category);
		void StartLoading(Enums.LoadingType type, Enums.LoadingCategory category);
		void StopLoading(Enums.LoadingType type, Enums.LoadingCategory category);
	}
}