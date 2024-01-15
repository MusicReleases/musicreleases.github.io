using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Objects.User.Spotify;

public class UserSettingsSpotify
{
	public bool PlaylistNewTrackPositionLast { get; set; } = true; // last or first position in playlist
	public bool NotificationsEnabled { get; set; } = true;
	public bool OpenLinksInApp { get; set; } = true;
	public SpotifyPlaylist? FavouritePlaylist { get; set; }
	public Theme Theme { get; set; } = Theme.System;
	public PlaylistVisiblity PlaylistVisiblity { get; set; } = PlaylistVisiblity.Default;
}
