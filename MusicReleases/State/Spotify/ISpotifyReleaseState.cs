using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.State.Spotify
{
	public interface ISpotifyReleaseState
	{
		event Action? OnChange;

		SpotifyRelease? GetById(string id);
		ConcurrentDictionary<MainReleasesType, IReadOnlyList<SpotifyRelease>> GetByType();
		IReadOnlyList<SpotifyRelease> GetSorted(MainReleasesType releaseType);
		void Set(MainReleasesType releaseType, IEnumerable<SpotifyRelease> releases);
	}
}