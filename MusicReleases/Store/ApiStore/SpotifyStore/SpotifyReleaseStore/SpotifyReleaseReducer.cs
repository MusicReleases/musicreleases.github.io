using Fluxor;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleaseStore;

public class SpotifyReleaseReducer
{
	// get storage
	[ReducerMethod(typeof(SpotifyReleaseActionGetStorage))]
	public static SpotifyReleaseState OnReleasesGetStorage(SpotifyReleaseState state)
	{
		return state with
		{
			LoadingStorage = true,
		};
	}
	[ReducerMethod(typeof(SpotifyReleaseActionGetStorageSuccess))]
	public static SpotifyReleaseState OnReleasesGetStorageSuccess(SpotifyReleaseState state)
	{
		return state with
		{
			LoadingStorage = false,
		};
	}
	[ReducerMethod(typeof(SpotifyReleaseActionGetStorageFailure))]
	public static SpotifyReleaseState OnReleasesGetStorageFailure(SpotifyReleaseState state)
	{
		return state with
		{
			Error = true,
			LoadingStorage = false,
		};
	}
	// get api
	[ReducerMethod(typeof(SpotifyReleaseActionGetApi))]
	public static SpotifyReleaseState OnReleasesGetApi(SpotifyReleaseState state)
	{
		return state with
		{
			LoadingApi = true,
		};
	}
	[ReducerMethod(typeof(SpotifyReleaseActionGetApiSuccess))]
	public static SpotifyReleaseState OnReleasesApiGetSucces(SpotifyReleaseState state)
	{
		return state with
		{
			LoadingApi = false,
		};
	}
	[ReducerMethod(typeof(SpotifyReleaseActionGetApiFailure))]
	public static SpotifyReleaseState OnReleasesGetApiFailure(SpotifyReleaseState state)
	{
		return state with
		{
			Error = true,
			LoadingApi = false,
		};
	}
	// set
	[ReducerMethod]
	public static SpotifyReleaseState OnSetReleases(SpotifyReleaseState state, SpotifyReleaseActionSet action)
	{
		return state with
		{
			List = action.Releases,
			NewReleases = action.NewReleases,
		};
	}

	// clear
	[ReducerMethod]
	public static SpotifyReleaseState OnClearReleasesNew(SpotifyReleaseState state, SpotifyReleaseNewActionClear action)
	{
		return state with
		{
			NewReleases = new HashSet<SpotifyRelease>(),
		};
	}

	// TODO persist state
	[ReducerMethod]
	public static SpotifyReleaseState OnSetReleasesFromStorage(SpotifyReleaseState state, SpotifyReleaseActionSetStorageState action)
	{
		return action.ReleasesState;
	}

	// TODO persist state
}
