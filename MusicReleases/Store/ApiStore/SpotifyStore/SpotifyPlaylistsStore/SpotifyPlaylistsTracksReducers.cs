using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;

public static class SpotifyPlaylistsTracksReducers
{
	// get api
	[ReducerMethod(typeof(SpotifyPlaylistsTracksActionGetApi))]
	public static SpotifyPlaylistsTracksState OnPlaylistGetApi(SpotifyPlaylistsTracksState state)
	{
		return state with
		{
			LoadingApi = true,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistsTracksActionGetApiSuccess))]
	public static SpotifyPlaylistsTracksState OnPlaylistApiGetSucces(SpotifyPlaylistsTracksState state)
	{
		return state with
		{
			LoadingApi = false,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistsTracksActionGetApiFailure))]
	public static SpotifyPlaylistsTracksState OnPlaylistGetApiFailure(SpotifyPlaylistsTracksState state)
	{
		return state with
		{
			Error = true,
			LoadingApi = false,
		};
	}

	// TODO persist state
	[ReducerMethod]
	public static SpotifyPlaylistsTracksState OnSetArtistsFromStorage(SpotifyPlaylistsTracksState state, SpotifyPlaylistsTracksActionSetStorageState action)
	{
		return action.PlaylistsTracksState;
	}
}
