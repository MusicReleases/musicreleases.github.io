using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.State.Spotify
{
	public interface ISpotifyReleaseState
	{
		ConcurrentDictionary<MainReleasesType, IReadOnlyList<SpotifyRelease>> ReleasesByType { get; }

		event Action? OnChange;
		ConcurrentDictionary<MainReleasesType, IReadOnlyList<SpotifyRelease>> GetByType();
		void Set(MainReleasesType releaseType, IEnumerable<SpotifyRelease> releases);
	}
}