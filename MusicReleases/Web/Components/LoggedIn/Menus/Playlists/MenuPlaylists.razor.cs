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
	private ISet<SpotifyPlaylist>? PlaylistsFiltered => (PlaylistNameTrimmed.IsNotNullOrEmpty() && Playlists is not null) ? Playlists.Where(x => x.Name.Contains(PlaylistNameTrimmed, StringComparison.CurrentCultureIgnoreCase)).ToHashSet() : Playlists;
	private bool Loading => LoaderService.IsLoading(LoadingType.Playlists);
	private string DivClass => Release is null ? "menu items scroll buttons-rounded-m" : "icon-text list";

	private string? _playlistName;
	private string? PlaylistNameTrimmed => _playlistName?.Trim();


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