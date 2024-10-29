using Fluxor;
using JakubKastner.MusicReleases.Controllers.DatabaseControllers;
using JakubKastner.SpotifyApi.Controllers;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;

public class SpotifyArtistsEffects(ISpotifyControllerArtist spotifyControllerArtist, IDatabaseArtistsController databaseController, ISpotifyControllerUser spotifyControllerUser)
{
	private readonly ISpotifyControllerArtist _spotifyControllerArtist = spotifyControllerArtist;
	private readonly IDatabaseArtistsController _databaseController = databaseController;
	private readonly ISpotifyControllerUser _spotifyControllerUser = spotifyControllerUser;

	// GET
	[EffectMethod]
	public async Task Get(SpotifyArtistsActionGet action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		dispatcher.Dispatch(new SpotifyArtistsActionGetStorage(action.ForceUpdate) { CompletionSource = action.CompletionSource });
	}
	[EffectMethod]
	public async Task GetSuccess(SpotifyArtistsActionGetSuccess action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		action.CompletionSource.SetResult(true);
	}
	[EffectMethod]
	public async Task GetFailure(SpotifyArtistsActionGetFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//#if DEBUG
		Console.WriteLine(action.ErrorMessage);
		//#endif

		action.CompletionSource.SetResult(false);
	}

	[EffectMethod]
	public async Task GetStorage(SpotifyArtistsActionGetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// clear new artists
			dispatcher.Dispatch(new SpotifyArtistsNewActionClear());

			// get item from storage
			//var artists = await _localStorageService.GetItemAsync<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>>(_localStorageName);

			//var artists = await _databaseController.GetArtists();
			var userId = _spotifyControllerUser.GetUserIdRequired();
			var artists = await _databaseController.GetFollowed(userId, true);

			if (artists is not null)
			{
				dispatcher.Dispatch(new SpotifyArtistsActionSet(artists));
			}
			dispatcher.Dispatch(new SpotifyArtistsActionGetStorageSuccess());
			dispatcher.Dispatch(new SpotifyArtistsActionGetApi(artists, action.ForceUpdate) { CompletionSource = action.CompletionSource });
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistsActionGetStorageFailure());
			dispatcher.Dispatch(new SpotifyArtistsActionGetFailure(ex.Message) { CompletionSource = action.CompletionSource });
		}
	}

	[EffectMethod]
	public async Task GetApi(SpotifyArtistsActionGetApi action, IDispatcher dispatcher)
	{
		try
		{
			// get item from api
			var artistsStorage = action.Artists;
			var artists = await _spotifyControllerArtist.GetUserFollowedArtists(artistsStorage, action.ForceUpdate);

			var newArtists = new HashSet<SpotifyArtist>();
			if (artistsStorage?.List is not null && artists.List is not null)
			{
				newArtists = artists.List.Except(artistsStorage.List).ToHashSet();
			}

			dispatcher.Dispatch(new SpotifyArtistsActionGetApiSuccess());

			dispatcher.Dispatch(new SpotifyArtistsActionSet(artists) { NewArtists = newArtists });
			dispatcher.Dispatch(new SpotifyArtistsActionSetStorage(artists));

			dispatcher.Dispatch(new SpotifyArtistsActionGetSuccess() { CompletionSource = action.CompletionSource });
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistsActionGetApiFailure());
			dispatcher.Dispatch(new SpotifyArtistsActionGetFailure(ex.Message) { CompletionSource = action.CompletionSource });
		}
	}

	// SET
	[EffectMethod]
	public async Task SetStorage(SpotifyArtistsActionSetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			//await _localStorageService.SetItemAsync(_localStorageName, action.Artists);*/
			//await _databaseController.SaveArtists(action.Artists);
			var userId = _spotifyControllerUser.GetUserIdRequired();
			await _databaseController.SaveArtists(userId, action.Artists);

			dispatcher.Dispatch(new SpotifyArtistsActionSetStorageSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistsActionSetStorageFailure(ex.Message));
		}
	}
	[EffectMethod]
	public async Task SetStorageFailure(SpotifyArtistsActionSetStorageFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

#if DEBUG
		Console.WriteLine(action.ErrorMessage);
#endif
	}

	// TODO PERSIST STATE (copy from playlists)
}
