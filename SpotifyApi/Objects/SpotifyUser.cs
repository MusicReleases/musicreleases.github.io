﻿using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUser
{
	public SpotifyUserInfo? Info { get; set; }
	public SpotifyUserCredentials? Credentials { get; set; }

	public SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? Playlists { get; set; }
	public SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>? FollowedArtists { get; set; }

	public SpotifyUser() { }

	public SpotifyUser(PrivateUser userApi, SpotifyUserCredentials credentials)
	{
		Info = new(userApi);
		Credentials = credentials;
	}

	public SpotifyUser(SpotifyUserInfo? info, SpotifyUserCredentials credentials)
	{
		Info = info;
		Credentials = credentials;
	}
}
