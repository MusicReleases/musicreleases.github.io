using Fluxor;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Playlists;

public class SpotifyPlaylistsEffects
{
	private readonly ISpotifyControllerPlaylist _spotifyControllerPlaylist;

	public SpotifyPlaylistsEffects(ISpotifyControllerPlaylist spotifyControllerPlaylist)
	{
		_spotifyControllerPlaylist = spotifyControllerPlaylist;
	}

	[EffectMethod(typeof(SpotifyPlaylistsActionLoad))]
	public async Task LoadPlaylists(IDispatcher dispatcher)
	{
		//dispatcher.Dispatch(new LoaderAction(true));
		var playlists = await _spotifyControllerPlaylist.GetUserPlaylists();
		dispatcher.Dispatch(new SpotifyPlaylistsActionSet(playlists));
		//dispatcher.Dispatch(new LoaderAction(false));
	}
}
