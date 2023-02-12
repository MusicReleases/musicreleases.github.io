using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Artists;

public class SpotifyArtistsActionSet
{
	public SortedSet<Artist> Artists { get; }

	public SpotifyArtistsActionSet(SortedSet<Artist> artists)
	{
		Artists = artists;
	}
}