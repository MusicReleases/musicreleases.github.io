using JakubKastner.Extensions;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;
using static JakubKastner.MusicReleases.Base.Enums;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class MenuPlaylists
{
	[Parameter]
	public SpotifyRelease? Release { get; set; }
	[Parameter]
	public SpotifyTrack? Track { get; set; }

	private IReadOnlyList<SpotifyPlaylist>? Playlists => FilterService.FilteredPlaylists;

	private string PlaylistName
	{
		get => FilterService.SearchText;
		set => FilterService.SetSearchText(value);
	}

	private bool Loading => LoaderService.IsLoading(LoadingType.Playlists);
	private string DivClass => Release is null && Track is null ? "menu items scroll buttons-rounded-m" : "icon-text list";

	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += StateHasChanged;
		FilterService.OnFilterChanged += StateHasChanged;
		base.OnInitialized();

		var userLoggedIn = ApiLoginService.IsUserLoggedIn();

		if (!userLoggedIn)
		{
			return;
		}

		FilterService.SetTypeFilter(PlaylistType.Editable);
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= StateHasChanged;
		FilterService.OnFilterChanged -= StateHasChanged;
	}


	private void ClearInput()
	{
		FilterService.SetSearchText(string.Empty);
	}

	private async Task CreatePlaylist()
	{
		if (PlaylistName.IsNullOrEmpty())
		{
			return;
		}

		await SpotifyPlaylistService.CreatePlaylist(PlaylistName);
		ClearInput();
	}
}