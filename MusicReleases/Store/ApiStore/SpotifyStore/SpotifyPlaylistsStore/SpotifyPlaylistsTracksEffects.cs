﻿using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;

public class SpotifyPlaylistsTracksEffects(ISpotifyControllerPlaylist spotifyControllerPlaylist)
{
	private readonly ISpotifyControllerPlaylist _spotifyControllerPlaylist = spotifyControllerPlaylist;

	// GET
	[EffectMethod]
	public async Task Get(SpotifyPlaylistsTracksActionGet action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		if (action.Playlists is null)
		{
			// provided playlists
			dispatcher.Dispatch(new SpotifyPlaylistsActionGetStorage(action.ForceUpdate) { CompletionSource = action.CompletionSource });
			return;
		}
		dispatcher.Dispatch(new SpotifyPlaylistsTracksActionGetApi(action.Playlists, action.ForceUpdate) { CompletionSource = action.CompletionSource });
	}
	[EffectMethod]
	public async Task GetSuccess(SpotifyPlaylistsTracksActionGetSuccess action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		action.CompletionSource.SetResult(true);
	}
	[EffectMethod]
	public async Task GetFailure(SpotifyPlaylistsTracksActionGetFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//#if DEBUG
		Console.WriteLine(action.ErrorMessage);
		//#endif
		action.CompletionSource.SetResult(false);
	}


	[EffectMethod]
	public async Task GetApi(SpotifyPlaylistsTracksActionGetApi action, IDispatcher dispatcher)
	{
		try
		{
			// get item from api
			var playlistsStorage = action.Playlists;
			var playlistsApi = await _spotifyControllerPlaylist.GetPlaylistsTracks(playlistsStorage, action.ForceUpdate);
			dispatcher.Dispatch(new SpotifyPlaylistsTracksActionGetApiSuccess());

			dispatcher.Dispatch(new SpotifyPlaylistsActionSet(playlistsApi));
			dispatcher.Dispatch(new SpotifyPlaylistsActionSetStorage(playlistsApi));
			dispatcher.Dispatch(new SpotifyPlaylistsTracksActionGetSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsTracksActionGetApiFailure());
			dispatcher.Dispatch(new SpotifyPlaylistsTracksActionGetFailure(ex.Message));
		}
	}
}
