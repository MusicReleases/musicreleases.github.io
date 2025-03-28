using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistStore;

public static class SpotifyPlaylistReducer
{
	// get storage
	[ReducerMethod(typeof(SpotifyPlaylistActionGetStorage))]
	public static SpotifyPlaylistState OnPlaylistGetStorage(SpotifyPlaylistState state)
	{
		return state with
		{
			LoadingStorage = true,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistActionGetStorageSuccess))]
	public static SpotifyPlaylistState OnPlaylistGetStorageSuccess(SpotifyPlaylistState state)
	{
		return state with
		{
			LoadingStorage = false,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistActionGetStorageFailure))]
	public static SpotifyPlaylistState OnPlaylistGetStorageFailure(SpotifyPlaylistState state)
	{
		return state with
		{
			Error = true,
			LoadingStorage = false,
		};
	}

	// get api
	[ReducerMethod(typeof(SpotifyPlaylistActionGetApi))]
	public static SpotifyPlaylistState OnPlaylistGetApi(SpotifyPlaylistState state)
	{
		return state with
		{
			LoadingApi = true,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistActionGetApiSuccess))]
	public static SpotifyPlaylistState OnPlaylistApiGetSucces(SpotifyPlaylistState state)
	{
		return state with
		{
			LoadingApi = false,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistActionGetApiFailure))]
	public static SpotifyPlaylistState OnPlaylistGetApiFailure(SpotifyPlaylistState state)
	{
		return state with
		{
			Error = true,
			LoadingApi = false,
		};
	}

	// set
	[ReducerMethod]
	public static SpotifyPlaylistState OnPlaylistSet(SpotifyPlaylistState state, SpotifyPlaylistActionSet action)
	{
		return state with
		{
			List = action.Playlists,
		};
	}

	// TODO persist state
	[ReducerMethod]
	public static SpotifyPlaylistState OnPlaylistSetFromStorage(SpotifyPlaylistState state, SpotifyPlaylistActionSetStorageState action)
	{
		return action.PlaylistsState;
	}
}
