using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.State.Spotify;

public class SpotifyReleaseState : ISpotifyReleaseState
{
	public event Action? OnChange;

	public ConcurrentDictionary<ReleaseEnums, IReadOnlyList<SpotifyRelease>> ReleasesByType { get; private set; } = new();

	public ConcurrentDictionary<ReleaseEnums, DateTime> LastSyncByType { get; private set; } = new();


	public void Set(ReleaseEnums releaseType, IEnumerable<SpotifyRelease> releases, DateTime lastSync)
	{
		var sorted = releases.OrderByDescending(r => r.ReleaseDate).ToList().AsReadOnly();

		ReleasesByType[releaseType] = sorted;
		LastSyncByType[releaseType] = lastSync;

		NotifyStateChanged();
	}

	private void NotifyStateChanged() => OnChange?.Invoke();
}