using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

public static class SpotifyPlaylistsReducers
{
	// get storage
	[ReducerMethod(typeof(SpotifyPlaylistsActionGetStorage))]
	public static SpotifyPlaylistsState OnPlaylistGetStorage(SpotifyPlaylistsState state)
	{
		return state with
		{
			LoadingStorage = true,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistsActionGetStorageSuccess))]
	public static SpotifyPlaylistsState OnPlaylistGetStorageSuccess(SpotifyPlaylistsState state)
	{
		return state with
		{
			LoadingStorage = false,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistsActionGetStorageFailure))]
	public static SpotifyPlaylistsState OnPlaylistGetStorageFailure(SpotifyPlaylistsState state)
	{
		return state with
		{
			Error = true,
			LoadingStorage = false,
		};
	}

	// get api
	[ReducerMethod(typeof(SpotifyPlaylistsActionGetApi))]
	public static SpotifyPlaylistsState OnPlaylistGetApi(SpotifyPlaylistsState state)
	{
		return state with
		{
			LoadingApi = true,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistsActionGetApiSuccess))]
	public static SpotifyPlaylistsState OnPlaylistApiGetSucces(SpotifyPlaylistsState state)
	{
		return state with
		{
			LoadingApi = false,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistsActionGetApiFailure))]
	public static SpotifyPlaylistsState OnPlaylistGetApiFailure(SpotifyPlaylistsState state)
	{
		return state with
		{
			Error = true,
			LoadingApi = false,
		};
	}

	// set
	[ReducerMethod]
	public static SpotifyPlaylistsState OnSetPlaylists(SpotifyPlaylistsState state, SpotifyPlaylistsActionSet action)
	{
		return state with
		{
			List = action.Playlists,
		};
	}

	// TODO persist state
	[ReducerMethod]
	public static SpotifyPlaylistsState OnSetArtistsFromStorage(SpotifyPlaylistsState state, SpotifyPlaylistsActionSetStorageState action)
	{
		return action.PlaylistsState;
	}
}
