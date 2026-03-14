using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.State.Spotify
{
	public interface ISpotifyReleaseState
	{
		ConcurrentDictionary<ReleaseEnums, IReadOnlyList<SpotifyRelease>> ReleasesByType { get; }
		ConcurrentDictionary<ReleaseEnums, DateTime> LastSyncByType { get; }

		event Action? OnChange;
		void Set(ReleaseEnums releaseType, IEnumerable<SpotifyRelease> releases, DateTime lastSync);
	}
}