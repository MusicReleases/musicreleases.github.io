using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;

public static class SpotifyPlaylistTrackReducers
{
	// get api
	[ReducerMethod(typeof(SpotifyPlaylistTrackActionGetApi))]
	public static SpotifyPlaylistTrackState OnPlaylistTrackGetApi(SpotifyPlaylistTrackState state)
	{
		return state with
		{
			LoadingApi = true,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistTrackActionGetApiSuccess))]
	public static SpotifyPlaylistTrackState OnPlaylistTrackApiGetSucces(SpotifyPlaylistTrackState state)
	{
		return state with
		{
			LoadingApi = false,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistTrackActionGetApiFailure))]
	public static SpotifyPlaylistTrackState OnPlaylistTrackGetApiFailure(SpotifyPlaylistTrackState state)
	{
		return state with
		{
			Error = true,
			LoadingApi = false,
		};
	}

	// TODO persist state
	[ReducerMethod]
	public static SpotifyPlaylistTrackState OnSetPlaylistTrackFromStorage(SpotifyPlaylistTrackState state, SpotifyPlaylistTrackActionSetStorageState action)
	{
		return action.PlaylistTrackState;
	}
}
