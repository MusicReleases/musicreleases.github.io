using JakubKastner.SpotifyApi.Artists;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal sealed class SpotifyArtistState : SpotifyState<SpotifyArtist>, ISpotifyArtistState
{
	protected override SpotifyArtist PreserveUserFlags(SpotifyArtist oldModel, SpotifyArtist incoming)
	{
		incoming.New = oldModel.New;
		return incoming;
	}
}