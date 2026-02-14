using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseActions
{
	[Parameter, EditorRequired]
	public required SpotifyRelease SpotifyRelease { get; set; }


	private string PlaylistTitle => _isPlaylistDisplayed ? "Hide playlists" : "Add release to playlist";

	private string PlaylistClass => $"rounded-l{(_isPlaylistDisplayed ? " active" : string.Empty)}";

	private string TracklistTitle => _isTracklistDisplayed ? "Hide tracklist" : "View tracklist";

	private string TracklistClass => $"rounded-l{(_isTracklistDisplayed ? " active" : string.Empty)}";

	private LucideIcon PlaylistIcon => _loadingPlaylists ? LucideIcon.LoaderCircle : LucideIcon.Plus;

	private LucideIcon TracklistIcon => _loadingTracklist ? LucideIcon.LoaderCircle : LucideIcon.ListMusic;


	private bool _isPlaylistDisplayed = false;

	private bool _isTracklistDisplayed = false;

	private bool _loadingPlaylists = false;

	private bool _loadingTracklist = false;


	private async Task ViewPlaylists()
	{
		_loadingPlaylists = true;
		_isTracklistDisplayed = false;
		await GetTracks();
		_isPlaylistDisplayed = !_isPlaylistDisplayed;
		_loadingPlaylists = false;
	}

	private async Task ViewTracklist()
	{
		_loadingTracklist = true;
		_isPlaylistDisplayed = false;
		// get tracks
		await GetTracks();
		_isTracklistDisplayed = !_isTracklistDisplayed;
		_loadingTracklist = false;
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
