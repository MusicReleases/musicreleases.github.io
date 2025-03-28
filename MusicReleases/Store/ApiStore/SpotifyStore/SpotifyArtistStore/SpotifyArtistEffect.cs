using Fluxor;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistStore;

public class SpotifyArtistEffect(ISpotifyArtistService spotifyArtistService, ISpotifyUserService spotifyUserService, IDbSpotifyUserArtistService dbUserArtistService)
{
	private readonly ISpotifyArtistService _spotifyArtistService = spotifyArtistService;
	private readonly ISpotifyUserService _spotifyUserService = spotifyUserService;
	private readonly IDbSpotifyUserArtistService _dbUserArtistService = dbUserArtistService;

	// GET
	[EffectMethod]
	public async Task Get(SpotifyArtistActionGet action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		dispatcher.Dispatch(new SpotifyArtistActionGetStorage(action.ForceUpdate)
		{
			CompletionSource = action.CompletionSource
		});
	}
	[EffectMethod]
	public async Task GetSuccess(SpotifyArtistActionGetSuccess action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		action.CompletionSource.SetResult(true);
	}
	[EffectMethod]
	public async Task GetFailure(SpotifyArtistActionGetFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//#if DEBUG
		Console.WriteLine(action.ErrorMessage);
		//#endif

		action.CompletionSource.SetResult(false);
	}

	[EffectMethod]
	public async Task GetStorage(SpotifyArtistActionGetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// clear new artists
			dispatcher.Dispatch(new SpotifyArtistNewActionClear());

			// get item from storage
			//var artists = await _localStorageService.GetItemAsync<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>>(_localStorageName);

			//var artists = await _databaseController.GetArtists();
			var userId = _spotifyUserService.GetUserIdRequired();
			//var artists = await _databaseController.GetFollowed(userId, true);
			var artists = await _dbUserArtistService.Get(userId);

			if (artists is not null)
			{
				dispatcher.Dispatch(new SpotifyArtistActionSet(artists));
			}
			dispatcher.Dispatch(new SpotifyArtistActionGetStorageSuccess());
			dispatcher.Dispatch(new SpotifyArtistActionGetApi(artists, action.ForceUpdate) { CompletionSource = action.CompletionSource });
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistActionGetStorageFailure());
			dispatcher.Dispatch(new SpotifyArtistActionGetFailure(ex.Message) { CompletionSource = action.CompletionSource });
		}
	}

	[EffectMethod]
	public async Task GetApi(SpotifyArtistActionGetApi action, IDispatcher dispatcher)
	{
		try
		{
			// get item from api
			var artistsStorage = action.Artists;
			var artists = await _spotifyArtistService.GetUserFollowedArtists(artistsStorage, action.ForceUpdate);

			var newArtists = new HashSet<SpotifyArtist>();
			if (artistsStorage?.List is not null && artists.List is not null)
			{
				newArtists = artists.List.Except(artistsStorage.List).ToHashSet();
			}

			dispatcher.Dispatch(new SpotifyArtistActionGetApiSuccess());

			dispatcher.Dispatch(new SpotifyArtistActionSet(artists) { NewArtists = newArtists });
			dispatcher.Dispatch(new SpotifyArtistActionSetStorage(artists));

			dispatcher.Dispatch(new SpotifyArtistActionGetSuccess() { CompletionSource = action.CompletionSource });
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistActionGetApiFailure());
			dispatcher.Dispatch(new SpotifyArtistActionGetFailure(ex.Message) { CompletionSource = action.CompletionSource });
		}
	}

	// SET
	[EffectMethod]
	public async Task SetStorage(SpotifyArtistActionSetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			//await _localStorageService.SetItemAsync(_localStorageName, action.Artists);*/
			//await _databaseController.SaveArtists(action.Artists);
			var userId = _spotifyUserService.GetUserIdRequired();
			//await _databaseControllerOld.SaveArtists(userId, action.Artists);
			//await _databaseController.SaveArtists(userId, action.Artists);
			await _dbUserArtistService.Save(userId, action.Artists);

			dispatcher.Dispatch(new SpotifyArtistActionSetStorageSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistActionSetStorageFailure(ex.Message));
		}
	}
	[EffectMethod]
	public async Task SetStorageFailure(SpotifyArtistActionSetStorageFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//#if DEBUG
		Console.WriteLine(action.ErrorMessage);
		//#endif
	}

	// TODO PERSIST STATE (copy from playlists)
}
