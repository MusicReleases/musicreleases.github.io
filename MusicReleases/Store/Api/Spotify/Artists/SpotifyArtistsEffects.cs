using Fluxor;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Artists;

public class SpotifyArtistsEffects
{
	private readonly ControllerArtist _spotifyControllerArtist;

	public SpotifyArtistsEffects(ControllerArtist spotifyControllerArtist)
	{
		_spotifyControllerArtist = spotifyControllerArtist;
	}

	[EffectMethod(typeof(SpotifyArtistsActionLoad))]
	public async Task LoadArtists(IDispatcher dispatcher)
	{
		var artists = await _spotifyControllerArtist.GetUserFollowedArtists();
		dispatcher.Dispatch(new SpotifyArtistsActionSet(artists));
	}
}
