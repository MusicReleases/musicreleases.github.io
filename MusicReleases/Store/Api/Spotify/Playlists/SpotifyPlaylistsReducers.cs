using Fluxor;
using JakubKastner.MusicReleases.Store.Releases;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Playlists;

public static class SpotifyPlaylistsReducers
{
	[ReducerMethod]
	public static SpotifyPlaylistsState OnSetForecasts(SpotifyPlaylistsState state, SpotifyPlaylistsActionSet action)
	{
		return state with
		{
			Playlists = action.Playlists,
			Loading = false,
		};
	}

	[ReducerMethod(typeof(SpotifyPlaylistsActionInitialized))]
	public static SpotifyPlaylistsState OnSetInitialized(SpotifyPlaylistsState state)
	{
		return state with
		{
			Initialized = true,
		};
	}

	[ReducerMethod(typeof(SpotifyPlaylistsActionLoad))]
	public static SpotifyPlaylistsState OnLoadForecasts(SpotifyPlaylistsState state)
	{
		return state with
		{
			Loading = true,
		};
	}
}
