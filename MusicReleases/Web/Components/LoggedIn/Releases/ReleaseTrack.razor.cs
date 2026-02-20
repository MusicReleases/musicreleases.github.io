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


	private string PlaylistButtonTitle => _isPlaylistListDisplayed ? "Hide playlists" : "Add track to playlist";

	private string PlaylistButtonClass => $"{_buttonClass}{(_isPlaylistListDisplayed ? " active" : string.Empty)}";

	private string ListClass => _isPlaylistListDisplayed ? string.Empty : "hidden";


	private const string _buttonClass = "track-info";

	private const string _artistButtonClass = $"{_buttonClass}-artist";

	private bool _renderPlaylistList = false;

	private bool _isPlaylistListDisplayed = false;


	private void OnDragStart()
	{
		DragDropService.StartDrag(Track, DragDropType.Track);
	}

	private void TogglePlaylists()
	{
		if (!_renderPlaylistList)
		{
			_renderPlaylistList = true;
		}

		_isPlaylistListDisplayed = !_isPlaylistListDisplayed;
	}
}
