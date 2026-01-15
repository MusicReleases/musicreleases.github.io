using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyFilterPlaylistService : IDisposable, ISpotifyFilterPlaylistService
{
	public SpotifyFilterPlaylistService(ISpotifyPlaylistState state, ISpotifyApiUserService userService)
	{
		_state = state;
		_userService = userService;

		_state.OnChange += ApplyFilter;
	}


	private readonly ISpotifyPlaylistState _state;
	private readonly ISpotifyApiUserService _userService;


	public IReadOnlyList<SpotifyPlaylist>? FilteredPlaylists { get; private set; }


	public string SearchText { get; private set; } = string.Empty;
	public PlaylistType TypeFilter { get; private set; } = PlaylistType.All;

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

	public void SetTypeFilter(PlaylistType type)
	{
		if (TypeFilter == type)
		{
			return;
		}
		TypeFilter = type;
		ApplyFilter();
	}

	private void ApplyFilter()
	{
		// check data
		if (_state.Playlists is null)
		{
			FilteredPlaylists = null;
			OnFilterChanged?.Invoke();
			return;
		}

		var userId = _userService.GetUserIdRequired();
		IEnumerable<SpotifyPlaylist> query = _state.Playlists;

		// filter by type
		if (TypeFilter != PlaylistType.All)
		{
			query = query.Where(p => (GetPlaylistType(p, userId) & TypeFilter) != 0);
		}

		// filter by text
		if (!string.IsNullOrWhiteSpace(SearchText))
		{
			var text = SearchText.Trim();
			if (text.StartsWith('"') && text.EndsWith('"') && text.Length > 1)
			{
				var exactPhrase = text.Length == 2 ? text : text[1..^1];
				query = query.Where(x => x.Name.Contains(exactPhrase, StringComparison.CurrentCultureIgnoreCase));
			}
			else
			{
				var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
				query = query.Where(p => words.All(word => p.Name.Contains(word, StringComparison.CurrentCultureIgnoreCase)));
			}
		}

		// save filtered playlist
		FilteredPlaylists = query.ToList().AsReadOnly();
		OnFilterChanged?.Invoke();
	}

	private static PlaylistType GetPlaylistType(SpotifyPlaylist playlist, string userId)
	{
		if (playlist.OwnerId == userId)
		{
			return PlaylistType.Owned;
		}
		if (playlist.Collaborative)
		{
			return PlaylistType.Collaborative;
		}
		return PlaylistType.Subscribed;
	}

	public void Dispose()
	{
		_state.OnChange -= ApplyFilter;
	}
}
