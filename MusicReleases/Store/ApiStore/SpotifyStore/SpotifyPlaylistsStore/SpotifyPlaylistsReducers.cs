﻿using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

public static class SpotifyPlaylistsReducers
{
	[ReducerMethod]
	public static SpotifyPlaylistsState OnSetPlaylists(SpotifyPlaylistsState state, SpotifyPlaylistsActionSet action)
	{
		return state with
		{
			List = action.Playlists,
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

	[ReducerMethod(typeof(SpotifyPlaylistsActionGet))]
	public static SpotifyPlaylistsState OnLoadPlaylists(SpotifyPlaylistsState state)
	{
		return state with
		{
			//Loading2 = true,
		};
	}

	[ReducerMethod(typeof(SpotifyPlaylistsActionGetStorage))]
	public static SpotifyPlaylistsState OnPlaylistStorageGet(SpotifyPlaylistsState state)
	{
		return state with
		{
			LoadingStorage = true,
		};
	}

	[ReducerMethod(typeof(SpotifyPlaylistsActionGetApi))]
	public static SpotifyPlaylistsState OnPlaylistApiGet(SpotifyPlaylistsState state)
	{
		return state with
		{
			LoadingApi = true,
		};
	}

	[ReducerMethod(typeof(SpotifyPlaylistsActionGetStorageSuccess))]
	public static SpotifyPlaylistsState OnPlaylistStorageGetSucces(SpotifyPlaylistsState state)
	{
		return state with
		{
			LoadingStorage = false,
		};
	}
	[ReducerMethod(typeof(SpotifyPlaylistsActionGetApiSuccess))]
	public static SpotifyPlaylistsState OnPlaylistApiGetSucces(SpotifyPlaylistsState state)
	{
		return state with
		{
			LoadingApi = false,
		};
	}
}
