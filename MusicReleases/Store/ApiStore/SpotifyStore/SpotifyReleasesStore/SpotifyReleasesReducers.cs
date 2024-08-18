using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

public class SpotifyReleasesReducers
{
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

	// TODO persist state
}
