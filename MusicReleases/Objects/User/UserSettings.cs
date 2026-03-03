using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Objects.User;

public class UserSettings
{
	public Theme Theme { get; set; } = Theme.System;

	public bool OpenLinksInApp { get; set; } = true;

	// last or first position in playlist
	public bool PlaylistNewTrackPositionLast { get; set; } = true;

	// add new playlist to user profile
	public bool PlaylistAddToProfile { get; set; } = false;


	/*public bool NotificationsEnabled { get; set; } = true;

	public bool MergeSameReleases { get; set; } = true;

	public bool PlaylistAutoDeduplication { get; set; } = true; // TODO prefer newest/oldest or albums/eps/singles/...
	public bool PlaylistAutoSort { get; set; } = true; // TODO custom sorting
	public SpotifyPlaylist? PlaylistFavourite { get; set; }*/

	public UserSettings()
	{

	}

	public UserSettings(Theme theme, bool openLinksInApp, bool playlistNewTrackPositionLast, bool playlistAddToProfile)
	{
		Theme = theme;
		OpenLinksInApp = openLinksInApp;
		PlaylistNewTrackPositionLast = playlistNewTrackPositionLast;
		PlaylistAddToProfile = playlistAddToProfile;
	}
}
