using JakubKastner.SpotifyApi.Objects.Base;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUser
{
	public SpotifyUserInfo? Info { get; set; }
	public SpotifyUserCredentials? Credentials { get; set; }

	public SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? Playlists { get; set; }
	public SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? FollowedArtists { get; set; }
	public SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? FollowedArtistReleases { get; set; }

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
