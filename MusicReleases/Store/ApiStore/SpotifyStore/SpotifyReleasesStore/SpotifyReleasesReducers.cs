using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

public class SpotifyReleasesReducers
{
	[ReducerMethod]
	public static SpotifyReleasesState OnSetPlaylists(SpotifyReleasesState state, SpotifyReleasesActionSet action)
	{
		return state with
		{
			Releases = action.Releases,
			Loading = false,
		};
	}

	[ReducerMethod(typeof(SpotifyReleasesActionInitialized))]
	public static SpotifyReleasesState OnSetInitialized(SpotifyReleasesState state)
	{
		return state with
		{
			Initialized = true,
		};
	}

	[ReducerMethod(typeof(SpotifyReleasesActionLoad))]
	public static SpotifyReleasesState OnLoadReleases(SpotifyReleasesState state)
	{
		return state with
		{
			Loading = true,
		};
	}
}
