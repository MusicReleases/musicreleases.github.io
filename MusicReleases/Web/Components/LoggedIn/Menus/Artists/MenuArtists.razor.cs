using JakubKastner.MusicReleases.Base;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Artists;

public partial class MenuArtists
{
	private SortedSet<SpotifyArtist>? _artists => _stateSpotifyArtists.Value.Artists;
	private bool _loading => _stateSpotifyArtists.Value.Loading;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (!_apiLoginController.IsUserLoggedIn())
		{
			return;
		}

		var serviceType = _apiLoginController.GetServiceType();

		if (serviceType == Enums.ServiceType.Spotify)
		{
			if (_stateSpotifyArtists.Value.Initialized == false)
			{
				LoadArtists();
				_dispatcher.Dispatch(new SpotifyArtistsActionInitialized());
			}
		}
	}

	private void LoadArtists()
	{
		// local storage
		_dispatcher.Dispatch(new SpotifyArtistsActionStorageGet());
		/*if (_stateSpotifyArtists.Value.Artists?.Count < 1)
			{
			// spotify api
			_dispatcher.Dispatch(new SpotifyArtistsActionLoad());
		}*/
	}

	private void LoadArtistsApi()
	{
		_dispatcher.Dispatch(new SpotifyArtistsActionLoad());
	}

	private void Save()
	{
		_dispatcher.Dispatch(new SpotifyArtistsActionStorageSet(_stateSpotifyArtists.Value));
	}
}