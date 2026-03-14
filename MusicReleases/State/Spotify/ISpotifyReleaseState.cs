using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.State.Spotify
{
	public interface ISpotifyReleaseState
	{
		ConcurrentDictionary<ReleaseGroup, IReadOnlyList<SpotifyRelease>> ReleasesByType { get; }
		ConcurrentDictionary<ReleaseGroup, DateTime> LastSyncByType { get; }

		event Action? OnChange;
		void Set(ReleaseGroup releaseType, IEnumerable<SpotifyRelease> releases, DateTime lastSync);
	}
}