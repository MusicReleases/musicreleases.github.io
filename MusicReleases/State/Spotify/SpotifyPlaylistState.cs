using JakubKastner.SpotifyApi.Objects;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.State.Spotify;

public class SpotifyPlaylistState : ISpotifyPlaylistState
{
	public event Action? OnChange;

	public IReadOnlyList<SpotifyPlaylist>? Playlists { get; private set; } = null;

	public DateTime? LastSync { get; private set; }


	private readonly ConcurrentDictionary<string, SpotifyPlaylist> _lookup = new();


	public void SetPlaylists(IEnumerable<SpotifyPlaylist> playlists, DateTime lastSync)
	{
		var playlistsList = playlists.ToList();
		Playlists = playlistsList.AsReadOnly();

		_lookup.Clear();
		foreach (var playlist in playlistsList)
		{
			_lookup[playlist.Id] = playlist;
		}
		LastSync = lastSync;

		NotifyStateChanged();
	}

	public void Add(SpotifyPlaylist playlist)
	{
		Playlists ??= [];
		var list = Playlists.ToList();
		list.Insert(0, playlist);
		Playlists = list.AsReadOnly();

		_lookup[playlist.Id] = playlist;
		NotifyStateChanged();
	}

	public SpotifyPlaylist? GetById(string id)
	{
		if (string.IsNullOrEmpty(id))
		{
			return null;
		}
		var playlist = _lookup.TryGetValue(id, out var p) ? p : null;
		return playlist;
	}

	private void NotifyStateChanged() => OnChange?.Invoke();
}