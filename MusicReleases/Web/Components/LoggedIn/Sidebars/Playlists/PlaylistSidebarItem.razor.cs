using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Playlists;

public partial class PlaylistSidebarItem
{
	[Inject]
	private IDragDropService DragDropService { get; set; } = default!;
	[Inject]
	private ISpotifyPlaylistService SpotifyPlaylistService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required string PlaylistName { get; set; }

	[Parameter, EditorRequired]
	public required string PlaylistId { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private bool _isLoading = false;


	private async Task HandleDrop()
	{
		_isLoading = true;
		var payload = DragDropService.Payload;

		if (payload is SpotifyTrack track)
		{
			await SpotifyPlaylistService.AddTrack(PlaylistId, track, true);
		}
		else if (payload is SpotifyRelease release)
		{
			// TODO add release to playlist
		}

		DragDropService.Reset();
		_isLoading = false;
	}
}