using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class MenuPlaylistsOld
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

		FilterService.SetTypeFilter(PlaylistType.Editable);
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= StateHasChanged;
		FilterService.OnFilterChanged -= StateHasChanged;
		GC.SuppressFinalize(this);
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