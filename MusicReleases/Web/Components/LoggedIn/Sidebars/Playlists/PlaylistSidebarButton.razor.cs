using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Playlists;

public partial class PlaylistSidebarButton : IDisposable
{
	[Inject]
	private IDragDropService DragDropService { get; set; } = default!;

	[Inject]
	private ISpotifyPlaylistService SpotifyPlaylistService { get; set; } = default!;

	[Inject]
	private ISpotifyTrackService SpotifyTracksService { get; set; } = default!;

	[Inject]
	private ISettingsService SettingsService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required SpotifyPlaylist Playlist { get; set; }


	private bool IsDragOver => _dragCounter > 0;

	private bool PositionTop => !SettingsService.UserSettings.PlaylistNewTrackPositionLast;


	private const string _buttonClass = "sidebar-content";

	private int _dragCounter = 0;


	protected override void OnInitialized()
	{
		SettingsService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		SettingsService.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}


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

		try
		{
			var payload = DragDropService.Payload;
			if (payload is SpotifyTrack track)
			{
				await SpotifyPlaylistService.AddTrack(Playlist.Id, track, PositionTop);
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

				await SpotifyPlaylistService.AddTracks(Playlist.Id, release.Tracks, PositionTop);
			}
		}
		finally
		{
			DragDropService.Reset();
		}
	}
}