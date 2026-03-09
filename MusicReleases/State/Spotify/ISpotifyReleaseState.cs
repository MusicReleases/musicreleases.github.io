using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.State.Spotify
{
	public interface ISpotifyReleaseState
	{
		ConcurrentDictionary<MainReleasesType, IReadOnlyList<SpotifyRelease>> ReleasesByType { get; }
		ConcurrentDictionary<MainReleasesType, DateTime> LastSyncByType { get; }

		event Action? OnChange;
		void Set(MainReleasesType releaseType, IEnumerable<SpotifyRelease> releases, DateTime lastSync);
	}
}