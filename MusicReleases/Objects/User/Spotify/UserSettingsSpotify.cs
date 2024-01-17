using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Objects.User.Spotify;

public class UserSettingsSpotify
{
	public bool NotificationsEnabled { get; set; } = true;
	public bool OpenLinksInApp { get; set; } = true;
	public Theme Theme { get; set; } = Theme.System;

	public bool MergeSameReleases { get; set; } = true;

	public bool PlaylistNewTrackPositionLast { get; set; } = true; // last or first position in playlist
	public bool PlaylistAutoDeduplication { get; set; } = true; // TODO prefer newest/oldest or albums/eps/singles/...
	public bool PlaylistAutoSort { get; set; } = true; // TODO custom sorting
	public SpotifyPlaylist? PlaylistFavourite { get; set; }
	public PlaylistVisiblity PlaylistVisiblity { get; set; } = PlaylistVisiblity.Default;
}
