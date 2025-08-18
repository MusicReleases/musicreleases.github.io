using JakubKastner.Extensions;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class MenuPlaylists
{
	[Parameter]
	public SpotifyRelease? Release { get; set; }

	private ISet<SpotifyPlaylist>? Playlists => SpotifyPlaylistsService.Playlists?.List;

	private ISet<SpotifyPlaylist>? PlaylistsFiltered
	{
		get
		{
			var playlistName = _playlistName?.Trim();

			if (playlistName.IsNullOrEmpty() || Playlists is null)
			{
				return Playlists;
			}

			if (playlistName.StartsWith('"') && playlistName.EndsWith('"') && playlistName.Length > 1)
			{
				var exactPhrase = playlistName.Length == 2 ? playlistName : playlistName[1..^1];

				return Playlists
					.Where(x => x.Name.Contains(exactPhrase, StringComparison.CurrentCultureIgnoreCase))
					.ToHashSet();
			}

			var words = playlistName.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

			return Playlists.Where(playlist => words.All(word => playlist.Name.Contains(word, StringComparison.CurrentCultureIgnoreCase))).ToHashSet();
		}
	}

	private bool Loading => LoaderService.IsLoading(LoadingType.Playlists);
	private string DivClass => Release is null ? "menu items scroll buttons-rounded-m" : "icon-text list";

	private string? _playlistName;


	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += LoadingStateChanged;
		SpotifyPlaylistsService.OnPlaylistsDataChanged += OnPlaylistsDataChanged;
		base.OnInitialized();

		var userLoggedIn = ApiLoginService.IsUserLoggedIn();

		if (!userLoggedIn)
		{
			return;
		}
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= LoadingStateChanged;
		SpotifyPlaylistsService.OnPlaylistsDataChanged -= OnPlaylistsDataChanged;
	}

	private void LoadingStateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
	private void OnPlaylistsDataChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void ClearInput()
	{
		_playlistName = string.Empty;
		//InvokeAsync(StateHasChanged);
	}

	private async Task CreatePlaylist()
	{
		// todo create playlist
	}
}