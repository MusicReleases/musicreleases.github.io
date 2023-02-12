using Fluxor;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Playlists;

public static class SpotifyPlaylistsReducers
{
	[ReducerMethod]
	public static SpotifyPlaylistsState OnSetPlaylists(SpotifyPlaylistsState state, SpotifyPlaylistsActionSet action)
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
	public static SpotifyPlaylistsState OnLoadPlaylists(SpotifyPlaylistsState state)
	{
		return state with
		{
			Loading = true,
		};
	}
}
