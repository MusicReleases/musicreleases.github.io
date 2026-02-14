using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Playlists;

public partial class PlaylistSidebarButton
{
	[Inject]
	private IDragDropService DragDropService { get; set; } = default!;

	[Inject]
	private ISpotifyPlaylistService SpotifyPlaylistService { get; set; } = default!;

	[Inject]
	private ISpotifyTracksService SpotifyTracksService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required string PlaylistName { get; set; }

	[Parameter, EditorRequired]
	public required string PlaylistId { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private bool IsDragOver => _dragCounter > 0;
	private string ButtonClass => $"rounded-m fill-width transparent {Class}{(IsDragOver ? " active" : string.Empty)}";


	private bool _isLoading = false;

	private readonly bool _positionTop = true;

	private int _dragCounter = 0;

	private void HandleDragEnter()
	{
		_dragCounter++;
	}

	private void HandleDragLeave()
	{
		if (IsDragOver)
		{
			_dragCounter--;
		}
	}

	private void HandleDragOver()
	{
		// empty, because we need to prevent default to allow dropping
	}

	private async Task HandleDrop()
	{
		_dragCounter = 0;
		_isLoading = true;
		var payload = DragDropService.Payload;

		if (payload is SpotifyTrack track)
		{
			await SpotifyPlaylistService.AddTrack(PlaylistId, track, _positionTop);
		}
		else if (payload is SpotifyRelease release)
		{
			if (release.Tracks is null || release.Tracks.Count < 1)
			{
				// try to get tracks
				await SpotifyTracksService.Get(release);
			}

			if (release.Tracks is null || release.Tracks.Count < 1)
			{
				return;
			}

			await SpotifyPlaylistService.AddTracks(PlaylistId, release.Tracks, _positionTop);
		}

		DragDropService.Reset();
		_isLoading = false;
	}
}