using Fluxor;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Artists;

public static class SpotifyArtistsReducers
{
	[ReducerMethod]
	public static SpotifyArtistsState OnSetArtists(SpotifyArtistsState state, SpotifyArtistsActionSet action)
	{
		return state with
		{
			Artists = new(action.Artists),
			Loading = false,
			Initialized = true,
		};
	}

	[ReducerMethod]
	public static SpotifyArtistsState OnSetArtistsFromStorage(SpotifyArtistsState state, SpotifyArtistsActionStorageSet action)
	{
		return action.ArtistsState;
	}

	/*[ReducerMethod(typeof(SpotifyArtistsActionInitialized))]
	public static SpotifyArtistsState OnSetInitialized(SpotifyArtistsState state)
	{
		return state with
		{
			Initialized = true,
		};
	}*/

	[ReducerMethod(typeof(SpotifyArtistsActionLoad))]
	public static SpotifyArtistsState OnLoadArtists(SpotifyArtistsState state)
	{
		return state with
		{
			Loading = true,
		};
	}
}
