namespace JakubKastner.SpotifyApi.Base;

public static class ApiRequestLimit
{
	public static int UserFollowedArtists { get; } = 50;
	public static int UserPlaylists { get; } = 50;
	public static int PlaylistsTracks { get; } = 50;
	public static int ArtistReleases { get; } = 50; // 10 -> https://developer.spotify.com/documentation/web-api/reference/get-an-artists-albums
	public static int ReleaseTracks { get; } = 50;
}
