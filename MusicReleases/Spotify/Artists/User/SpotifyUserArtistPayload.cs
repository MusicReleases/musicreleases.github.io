namespace JakubKastner.MusicReleases.Spotify.Artists.User;

public record SpotifyUserArtistPayload(string Id) : ISpotifyPayload, IComparable<SpotifyUserArtistPayload>
{
	public int CompareTo(SpotifyUserArtistPayload? other)
	{
		return string.Compare(Id, other?.Id, StringComparison.Ordinal);
	}
};