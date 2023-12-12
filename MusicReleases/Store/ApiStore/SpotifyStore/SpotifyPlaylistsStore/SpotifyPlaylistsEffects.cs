using Fluxor;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

public class SpotifyPlaylistsEffects(ISpotifyControllerPlaylist spotifyControllerPlaylist)
{
	private readonly ISpotifyControllerPlaylist _spotifyControllerPlaylist = spotifyControllerPlaylist;

	[EffectMethod(typeof(SpotifyPlaylistsActionLoad))]
	public async Task LoadPlaylists(IDispatcher dispatcher)
	{
		//dispatcher.Dispatch(new LoaderAction(true));
		var playlists = await _spotifyControllerPlaylist.GetUserPlaylists();
		dispatcher.Dispatch(new SpotifyPlaylistsActionSet(playlists));
		//dispatcher.Dispatch(new LoaderAction(false));
	}
}
