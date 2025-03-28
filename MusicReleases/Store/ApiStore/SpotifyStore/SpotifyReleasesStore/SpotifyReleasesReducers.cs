using Fluxor;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

public class SpotifyReleasesReducers
{
	// get storage
	[ReducerMethod(typeof(SpotifyReleasesActionGetStorage))]
	public static SpotifyReleasesState OnReleasesGetStorage(SpotifyReleasesState state)
	{
		return state with
		{
			LoadingStorage = true,
		};
	}
	[ReducerMethod(typeof(SpotifyReleasesActionGetStorageSuccess))]
	public static SpotifyReleasesState OnReleasesGetStorageSuccess(SpotifyReleasesState state)
	{
		return state with
		{
			LoadingStorage = false,
		};
	}
	[ReducerMethod(typeof(SpotifyReleasesActionGetStorageFailure))]
	public static SpotifyReleasesState OnReleasesGetStorageFailure(SpotifyReleasesState state)
	{
		return state with
		{
			Error = true,
			LoadingStorage = false,
		};
	}
	// get api
	[ReducerMethod(typeof(SpotifyReleasesActionGetApi))]
	public static SpotifyReleasesState OnReleasesGetApi(SpotifyReleasesState state)
	{
		return state with
		{
			LoadingApi = true,
		};
	}
	[ReducerMethod(typeof(SpotifyReleasesActionGetApiSuccess))]
	public static SpotifyReleasesState OnReleasesApiGetSucces(SpotifyReleasesState state)
	{
		return state with
		{
			LoadingApi = false,
		};
	}
	[ReducerMethod(typeof(SpotifyReleasesActionGetApiFailure))]
	public static SpotifyReleasesState OnReleasesGetApiFailure(SpotifyReleasesState state)
	{
		return state with
		{
			Error = true,
			LoadingApi = false,
		};
	}
	// set
	[ReducerMethod]
	public static SpotifyReleasesState OnSetReleases(SpotifyReleasesState state, SpotifyReleasesActionSet action)
	{
		return state with
		{
			List = action.Releases,
			NewReleases = action.NewReleases,
		};
	}

	// clear
	[ReducerMethod]
	public static SpotifyReleasesState OnClearReleasesNew(SpotifyReleasesState state, SpotifyReleasesNewActionClear action)
	{
		return state with
		{
			NewReleases = new HashSet<SpotifyRelease>(),
		};
	}

	// TODO persist state
	[ReducerMethod]
	public static SpotifyReleasesState OnSetReleasesFromStorage(SpotifyReleasesState state, SpotifyReleasesActionSetStorageState action)
	{
		return action.ReleasesState;
	}

	// TODO persist state
}
