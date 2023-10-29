using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUser
{
	public SpotifyUserInfo? Info { get; set; }
	public SpotifyUserCredentials? Credentials { get; set; }

	public ISet<SpotifyPlaylist>? Playlists { get; set; }
	public ISet<SpotifyArtist>? FollowedArtists { get; set; }
	public ISet<SpotifyAlbum> ReleasedAlbums { get; set; } = new SortedSet<SpotifyAlbum>();

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
