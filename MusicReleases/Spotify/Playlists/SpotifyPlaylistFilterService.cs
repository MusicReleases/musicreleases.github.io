using JakubKastner.SpotifyApi.Playlists;
using JakubKastner.SpotifyApi.User;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal class SpotifyPlaylistFilterService : ISpotifyPlaylistFilterService
{
	private readonly ISpotifyPlaylistState _playlistState;

	private readonly ISpotifyUserClient _userApi;

	public SpotifyPlaylistFilterService(ISpotifyPlaylistState playlistState, ISpotifyUserClient userApi)
	{
		_playlistState = playlistState;
		_userApi = userApi;

		_playlistState.OnChange += DataChanged;
	}

	public void Dispose()
	{
		_playlistState.OnChange -= DataChanged;
		GC.SuppressFinalize(this);
	}

	public string? SearchText { get; private set; }

	public IReadOnlySet<SpotifyPlaylist>? FilteredPlaylists { get; private set; }

	public SpotifyPlaylistType PlaylistType { get; private set; } = SpotifyPlaylistType.All;

	public event Action? OnSearchTextChanged;
	public event Action? OnFilterChanged;
	public event Action? OnChanged;

	public bool SetSearchText(string? searchText)
	{
		searchText = searchText.EnsureText();

		if (string.Equals(searchText, SearchText, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		SearchText = searchText;
		SearchChanged();
		return true;
	}

	public bool SetTypeFilter(SpotifyPlaylistType type)
	{
		if (PlaylistType == type)
		{
			return false;
		}
		PlaylistType = type;
		FilterChanged();
		return true;
	}

	private void DataChanged()
	{
		FilteredPlaylists = Recalculate(PlaylistType, SearchText);

		OnChanged?.Invoke();
	}

	private void SearchChanged()
	{
		FilteredPlaylists = Recalculate(PlaylistType, SearchText);

		OnSearchTextChanged?.Invoke();
		OnChanged?.Invoke();
	}

	private void FilterChanged()
	{
		FilteredPlaylists = ConvertToSet(RecalculateFilter(PlaylistType, FilteredPlaylists));

		OnFilterChanged?.Invoke();
		OnChanged?.Invoke();
	}

	private IReadOnlySet<SpotifyPlaylist>? ConvertToSet(IEnumerable<SpotifyPlaylist>? playlists)
	{
		if (playlists is null)
		{
			return null;
		}

		return new SortedSet<SpotifyPlaylist>(playlists).AsReadOnly();
	}

	private IReadOnlySet<SpotifyPlaylist>? Recalculate(SpotifyPlaylistType playlistType, string? searchText)
	{
		var playlists = _playlistState.Items;
		if (playlists is null)
		{
			return null;
		}

		var filtered = RecalculateFilter(playlistType, playlists);
		var searched = RecalculateSearch(searchText, filtered);

		return ConvertToSet(searched);
	}

	private IEnumerable<SpotifyPlaylist>? RecalculateSearch(string? searchText, IEnumerable<SpotifyPlaylist>? playlists = null)
	{
		playlists ??= _playlistState.Items;

		if (playlists is null)
		{
			return null;
		}

		searchText = searchText.EnsureText();

		if (searchText.IsNullOrEmpty())
		{
			return playlists;
		}

		var searched = playlists.ApplySearch(searchText, x => x.Name);

		return searched;
	}

	private IEnumerable<SpotifyPlaylist>? RecalculateFilter(SpotifyPlaylistType playlistType, IEnumerable<SpotifyPlaylist>? playlists = null)
	{
		playlists ??= _playlistState.Items;

		if (playlists is null)
		{
			return null;
		}

		if (playlistType == SpotifyPlaylistType.All)
		{
			return playlists;
		}

		var userId = _userApi.GetUserIdRequired();

		var filtered = playlists.Where(p => playlistType.HasFlag(GetPlaylistType(p, userId)));

		return filtered;
	}

	private static SpotifyPlaylistType GetPlaylistType(SpotifyPlaylist playlist, string userId)
	{
		if (playlist.OwnerId == userId)
		{
			return SpotifyPlaylistType.Owned;
		}

		if (playlist.Collaborative)
		{
			return SpotifyPlaylistType.Collaborative;
		}

		return SpotifyPlaylistType.Subscribed;

	}

	public IReadOnlySet<SpotifyPlaylist>? GetFilteredPlaylists(SpotifyPlaylistType playlistType, string? searchText)
	{
		return Recalculate(playlistType, searchText);
	}
}
