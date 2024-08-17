using Fluxor;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;

public static class SpotifyArtistsReducers
{
	// get storage
	[ReducerMethod(typeof(SpotifyArtistsActionGetStorage))]
	public static SpotifyArtistsState OnPlaylistGetStorage(SpotifyArtistsState state)
	{
		return state with
		{
			LoadingStorage = true,
		};
	}
	[ReducerMethod(typeof(SpotifyArtistsActionGetStorageSuccess))]
	public static SpotifyArtistsState OnPlaylistGetStorageSuccess(SpotifyArtistsState state)
	{
		return state with
		{
			LoadingStorage = false,
		};
	}
	[ReducerMethod(typeof(SpotifyArtistsActionGetStorageFailure))]
	public static SpotifyArtistsState OnPlaylistGetStorageFailure(SpotifyArtistsState state)
	{
		return state with
		{
			Error = true,
			LoadingStorage = false,
		};
	}

	// get api
	[ReducerMethod(typeof(SpotifyArtistsActionGetApi))]
	public static SpotifyArtistsState OnPlaylistGetApi(SpotifyArtistsState state)
	{
		return state with
		{
			LoadingApi = true,
		};
	}
	[ReducerMethod(typeof(SpotifyArtistsActionGetApiSuccess))]
	public static SpotifyArtistsState OnPlaylistApiGetSucces(SpotifyArtistsState state)
	{
		return state with
		{
			LoadingApi = false,
		};
	}
	[ReducerMethod(typeof(SpotifyArtistsActionGetApiFailure))]
	public static SpotifyArtistsState OnPlaylistGetApiFailure(SpotifyArtistsState state)
	{
		return state with
		{
			Error = true,
			LoadingApi = false,
		};
	}

	// set
	[ReducerMethod]
	public static SpotifyArtistsState OnSetArtists(SpotifyArtistsState state, SpotifyArtistsActionSet action)
	{
		return state with
		{
			List = action.Artists,
			NewArtists = action.NewArtists,
		};
	}

	// clear
	[ReducerMethod]
	public static SpotifyArtistsState OnClearArtistsNew(SpotifyArtistsState state, SpotifyArtistsNewActionClear action)
	{
		return state with
		{
			NewArtists = new HashSet<SpotifyArtist>(),
		};
	}

	// TODO persist state
	[ReducerMethod]
	public static SpotifyArtistsState OnSetArtistsFromStorage(SpotifyArtistsState state, SpotifyArtistsActionSetStorageState action)
	{
		return action.ArtistsState;
	}
}
