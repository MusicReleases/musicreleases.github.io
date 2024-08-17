using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

public class SpotifyReleasesReducers
{
	// get storage
	[ReducerMethod(typeof(SpotifyReleasesActionGetStorage))]
	public static SpotifyReleasesState OnPlaylistGetStorage(SpotifyReleasesState state)
	{
		return state with
		{
			LoadingStorage = true,
		};
	}
	[ReducerMethod(typeof(SpotifyReleasesActionGetStorageSuccess))]
	public static SpotifyReleasesState OnPlaylistGetStorageSuccess(SpotifyReleasesState state)
	{
		return state with
		{
			LoadingStorage = false,
		};
	}
	[ReducerMethod(typeof(SpotifyReleasesActionGetStorageFailure))]
	public static SpotifyReleasesState OnPlaylistGetStorageFailure(SpotifyReleasesState state)
	{
		return state with
		{
			Error = true,
			LoadingStorage = false,
		};
	}

	// get api
	[ReducerMethod(typeof(SpotifyReleasesActionGetApi))]
	public static SpotifyReleasesState OnPlaylistGetApi(SpotifyReleasesState state)
	{
		return state with
		{
			LoadingApi = true,
		};
	}
	[ReducerMethod(typeof(SpotifyReleasesActionGetApiSuccess))]
	public static SpotifyReleasesState OnPlaylistApiGetSucces(SpotifyReleasesState state)
	{
		return state with
		{
			LoadingApi = false,
		};
	}
	[ReducerMethod(typeof(SpotifyReleasesActionGetApiFailure))]
	public static SpotifyReleasesState OnPlaylistGetApiFailure(SpotifyReleasesState state)
	{
		return state with
		{
			Error = true,
			LoadingApi = false,
		};
	}

	// set
	[ReducerMethod]
	public static SpotifyReleasesState OnSetPlaylists(SpotifyReleasesState state, SpotifyReleasesActionSet action)
	{
		return state with
		{
			List = action.Releases,
		};
	}

	// TODO persist state
}
