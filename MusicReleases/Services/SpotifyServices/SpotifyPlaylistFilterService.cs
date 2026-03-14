using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Clients;
using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public class SpotifyPlaylistFilterService : IDisposable, ISpotifyFilterPlaylistService
{
	private readonly ISpotifyPlaylistState _state;

	private readonly ISpotifyUserClient _spotifyUserClient;

	public SpotifyPlaylistFilterService(ISpotifyPlaylistState state, ISpotifyUserClient spotifyUserClient)
	{
		_state = state;
		_spotifyUserClient = spotifyUserClient;

		_state.OnChange += ApplyFilter;
	}

	public IReadOnlyList<SpotifyPlaylist>? FilteredPlaylists { get; private set; }

	public string SearchText { get; private set; } = string.Empty;

	public PlaylistEnums FilterType { get; private set; } = PlaylistEnums.All;

	public event Action? OnFilterChanged;


	public void SetSearchText(string text)
	{
		if (SearchText == text)
		{
			return;
		}
		SearchText = text;
		ApplyFilter();
	}

	public void SetTypeFilter(PlaylistEnums type)
	{
		if (FilterType == type)
		{
			return;
		}
		FilterType = type;
		ApplyFilter();
	}

	private void ApplyFilter()
	{
		// filter and save
		FilteredPlaylists = GetFilteredPlaylists(SearchText, FilterType)?.ToList().AsReadOnly();
		OnFilterChanged?.Invoke();
	}

	public void Dispose()
	{
		_state.OnChange -= ApplyFilter;
		GC.SuppressFinalize(this);
	}


	public IEnumerable<SpotifyPlaylist>? GetFilteredPlaylists(string searchText, PlaylistEnums typeFilter = PlaylistEnums.All)
	{
		// check data
		var playlists = _state.Playlists;
		if (playlists is null)
		{
			return null;
		}

		IEnumerable<SpotifyPlaylist> query = playlists;
		var userId = _spotifyUserClient.GetUserIdRequired();

		// filter by type
		if (typeFilter != PlaylistEnums.All)
		{
			query = query.Where(p => (GetPlaylistType(p, userId) & typeFilter) != 0);
		}

		// filter by text
		query = query.ApplySearch(searchText, x => x.Name);

		return query;
	}

	private static PlaylistEnums GetPlaylistType(SpotifyPlaylist playlist, string userId)
	{
		if (playlist.OwnerId == userId)
		{
			return PlaylistEnums.Owned;
		}
		if (playlist.Collaborative)
		{
			return PlaylistEnums.Collaborative;
		}
		return PlaylistEnums.Subscribed;
	}
}
