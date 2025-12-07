using JakubKastner.SpotifyApi.Objects;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.State.Spotify;

public class SpotifyArtistState : ISpotifyArtistState
{
	public event Action? OnChange;

	public IReadOnlyList<SpotifyArtist> SortedFollowedArtists { get; private set; } = [];

	private readonly ConcurrentDictionary<string, SpotifyArtist> _lookup = new();

	public void SetFollowed(IEnumerable<SpotifyArtist> artists)
	{
		var list = artists.OrderBy(a => a.Name).ToList();
		SortedFollowedArtists = list.AsReadOnly();

		foreach (var a in list)
		{
			_lookup[a.Id] = a;
		}

		NotifyStateChanged();
	}

	public void MergeCache(IEnumerable<SpotifyArtist> others)
	{
		foreach (var a in others)
		{
			_lookup.TryAdd(a.Id, a);
		}
	}

	public SpotifyArtist? GetById(string id)
	{
		if (string.IsNullOrEmpty(id)) return null;
		return _lookup.TryGetValue(id, out var artist) ? artist : null;
	}
	private void NotifyStateChanged() => OnChange?.Invoke();
}