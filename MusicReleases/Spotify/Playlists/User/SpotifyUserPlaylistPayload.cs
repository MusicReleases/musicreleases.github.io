namespace JakubKastner.MusicReleases.Spotify.Playlists.User;

public record SpotifyUserPlaylistPayload(string Id, int Order) : ISpotifyPayload, IComparable<SpotifyUserPlaylistPayload>
{
	public int CompareTo(SpotifyUserPlaylistPayload? other)
	{
		return Order.CompareTo(other?.Order);
	}
};