using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseTrack
{
	[Parameter, EditorRequired]
	public required SpotifyTrack Track { get; set; }

	private bool _isPlaylistDisplayed = false;
	private string PlaylistTitle => _isPlaylistDisplayed ? "Hide playlists" : "Add track to playlist";
	private string PlaylistClass => _isPlaylistDisplayed ? "active" : string.Empty;

	private void ViewPlaylists()
	{
		_isPlaylistDisplayed = !_isPlaylistDisplayed;
	}
}
