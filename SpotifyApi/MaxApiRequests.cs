namespace JakubKastner.SpotifyApi;

public static class ApiRequestLimit
{
	public static int UserFollowedArtists { get; } = 50;
	public static int UserPlaylists { get; } = 50;
	public static int ArtistReleases { get; } = 50;
	public static int ReleaseTracks { get; } = 100;
}
