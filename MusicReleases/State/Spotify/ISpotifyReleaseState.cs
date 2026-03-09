using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
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