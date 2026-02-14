using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseTrack
{
	[Inject]
	private IDragDropService DragDropService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required SpotifyTrack Track { get; set; }


	private string PlaylistTitle => _isPlaylistDisplayed ? "Hide playlists" : "Add track to playlist";

	private string PlaylistClass => $"transparent rounded-l{(_isPlaylistDisplayed ? " active" : string.Empty)}";


	private bool _isPlaylistDisplayed = false;


	private void OnDragStart()
	{
		DragDropService.StartDrag(Track, DragDropType.Track);
	}

	private void ViewPlaylists()
	{
		_isPlaylistDisplayed = !_isPlaylistDisplayed;
	}
}
