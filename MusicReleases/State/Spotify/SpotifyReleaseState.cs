using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.State.Spotify;

public class SpotifyReleaseState : ISpotifyReleaseState
{
	public event Action? OnChange;

	public ConcurrentDictionary<MainReleasesType, IReadOnlyList<SpotifyRelease>> ReleasesByType { get; private set; } = new();

	public void Set(MainReleasesType releaseType, IEnumerable<SpotifyRelease> releases)
	{
		var sorted = releases.OrderByDescending(r => r.ReleaseDate).ToList().AsReadOnly();

		ReleasesByType[releaseType] = sorted;

		NotifyStateChanged();
	}

	public ConcurrentDictionary<MainReleasesType, IReadOnlyList<SpotifyRelease>> GetByType()
	{
		return ReleasesByType;
	}


	private void NotifyStateChanged() => OnChange?.Invoke();
}