using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseActions
{
	[Parameter, EditorRequired]
	public required SpotifyRelease SpotifyRelease { get; set; }

	private bool _isPlaylistDisplayed = false;
	private string PlaylistTitle => _isPlaylistDisplayed ? "Hide playlists" : "Add release to playlist";
	private string PlaylistClass => _isPlaylistDisplayed ? "active" : string.Empty;

	private bool _isTracklistDisplayed = false;
	private string TracklistTitle => _isTracklistDisplayed ? "Hide tracklist" : "View tracklist";
	private string TracklistClass => _isTracklistDisplayed ? "active" : string.Empty;

	private async Task ViewPlaylists()
	{
		_isTracklistDisplayed = false;
		await GetTracks();
		_isPlaylistDisplayed = !_isPlaylistDisplayed;
	}

	private async Task ViewTracklist()
	{
		_isPlaylistDisplayed = false;
		// get tracks
		await GetTracks();
		_isTracklistDisplayed = !_isTracklistDisplayed;
	}

	private async Task GetTracks()
	{
		if (SpotifyRelease.Tracks is not null && SpotifyRelease.Tracks.Count == 0)
		{
			// tracks loaded
			return;
		}
		await SpotifyTracksService.Get(SpotifyRelease);
	}
}
