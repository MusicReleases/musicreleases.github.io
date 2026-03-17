namespace JakubKastner.MusicReleases.Spotify.Artists.User;

internal record SpotifyUserArtistPayload(string Id) : ISpotifyPayload, IComparable<SpotifyUserArtistPayload>
{
	public int CompareTo(SpotifyUserArtistPayload? other)
	{
		return string.Compare(Id, other?.Id, StringComparison.Ordinal);
	}
};