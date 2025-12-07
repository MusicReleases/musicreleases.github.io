using JakubKastner.MusicReleases.Objects.Spotify;
using System.Collections.ObjectModel;

namespace JakubKastner.MusicReleases.Services.BaseServices
{
	public interface ISpotifyTaskManagerService
	{
		bool IsAnyTaskRunning { get; }
		ObservableCollection<SpotifyBackgroundTask> Tasks { get; }

		event Action? OnChange;

		Task Run(string name, Func<SpotifyBackgroundTask, Task> work);
	}
}