using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.State.Spotify;

public class SpotifyReleaseState : ISpotifyReleaseState
{
	public event Action? OnChange;

	private readonly ConcurrentDictionary<MainReleasesType, IReadOnlyList<SpotifyRelease>> _byType = new();

	private readonly ConcurrentDictionary<string, SpotifyRelease> _lookup = new();

	public void Set(MainReleasesType releaseType, IEnumerable<SpotifyRelease> releases)
	{
		var sorted = releases.OrderByDescending(r => r.ReleaseDate).ToList().AsReadOnly();

		_byType[releaseType] = sorted;

		foreach (var r in sorted)
		{
			_lookup[r.Id] = r;
		}

		NotifyStateChanged();
	}

	public IReadOnlyList<SpotifyRelease> GetSorted(MainReleasesType releaseType)
	{
		return _byType.TryGetValue(releaseType, out var list) ? list : [];
	}

	public ConcurrentDictionary<MainReleasesType, IReadOnlyList<SpotifyRelease>> GetByType()
	{
		return _byType;
	}

	public SpotifyRelease? GetById(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		return _lookup.TryGetValue(id, out var release) ? release : null;
	}

	private void NotifyStateChanged() => OnChange?.Invoke();
}