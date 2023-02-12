using Fluxor;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Artists;

public static class SpotifyArtistsReducers
{
	[ReducerMethod]
	public static SpotifyArtistsState OnSetArtists(SpotifyArtistsState state, SpotifyArtistsActionSet action)
	{
		return state with
		{
			Artists = action.Artists,
			Loading = false,
		};
	}

	[ReducerMethod(typeof(SpotifyArtistsActionInitialized))]
	public static SpotifyArtistsState OnSetInitialized(SpotifyArtistsState state)
	{
		return state with
		{
			Initialized = true,
		};
	}

	[ReducerMethod(typeof(SpotifyArtistsActionLoad))]
	public static SpotifyArtistsState OnLoadArtists(SpotifyArtistsState state)
	{
		return state with
		{
			Loading = true,
		};
	}
}
