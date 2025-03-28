using Fluxor;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistStore;

public static class SpotifyArtistReducer
{
	// get storage
	[ReducerMethod(typeof(SpotifyArtistActionGetStorage))]
	public static SpotifyArtistState OnArtistGetStorage(SpotifyArtistState state)
	{
		return state with
		{
			LoadingStorage = true,
		};
	}
	[ReducerMethod(typeof(SpotifyArtistActionGetStorageSuccess))]
	public static SpotifyArtistState OnArtistGetStorageSuccess(SpotifyArtistState state)
	{
		return state with
		{
			LoadingStorage = false,
		};
	}
	[ReducerMethod(typeof(SpotifyArtistActionGetStorageFailure))]
	public static SpotifyArtistState OnArtistGetStorageFailure(SpotifyArtistState state)
	{
		return state with
		{
			Error = true,
			LoadingStorage = false,
		};
	}

	// get api
	[ReducerMethod(typeof(SpotifyArtistActionGetApi))]
	public static SpotifyArtistState OnArtistGetApi(SpotifyArtistState state)
	{
		return state with
		{
			LoadingApi = true,
		};
	}
	[ReducerMethod(typeof(SpotifyArtistActionGetApiSuccess))]
	public static SpotifyArtistState OnArtistApiGetSucces(SpotifyArtistState state)
	{
		return state with
		{
			LoadingApi = false,
		};
	}
	[ReducerMethod(typeof(SpotifyArtistActionGetApiFailure))]
	public static SpotifyArtistState OnArtistGetApiFailure(SpotifyArtistState state)
	{
		return state with
		{
			Error = true,
			LoadingApi = false,
		};
	}

	// set
	[ReducerMethod]
	public static SpotifyArtistState OnArtistSet(SpotifyArtistState state, SpotifyArtistActionSet action)
	{
		return state with
		{
			List = action.Artists,
			NewArtists = action.NewArtists,
		};
	}

	// clear
	[ReducerMethod]
	public static SpotifyArtistState OnClearArtistNew(SpotifyArtistState state, SpotifyArtistNewActionClear action)
	{
		return state with
		{
			NewArtists = new HashSet<SpotifyArtist>(),
		};
	}

	// TODO persist state
	[ReducerMethod]
	public static SpotifyArtistState OnArtistSetFromStorage(SpotifyArtistState state, SpotifyArtistActionSetStorageState action)
	{
		return action.ArtistsState;
	}
}
