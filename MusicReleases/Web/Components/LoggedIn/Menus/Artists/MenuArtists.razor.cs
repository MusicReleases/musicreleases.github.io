using JakubKastner.MusicReleases.Base;
using JakubKastner.MusicReleases.Store.Api.Spotify.Artists;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Artists;

public partial class MenuArtists
{
    [Parameter]
    public Enums.ServiceType ServiceType { get; set; }
    private SortedSet<SpotifyArtist>? _artists => _stateSpotifyArtists.Value.Artists;
    private bool _loading => _stateSpotifyArtists.Value.Loading;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (ServiceType != Enums.ServiceType.Spotify)
        {
            return;
        }

        if (!_spotifyControllerUser.IsLoggedIn())
        {
            return;
        }

        if (_stateSpotifyArtists.Value.Initialized == false)
        {
            LoadArtists();
            _dispatcher.Dispatch(new SpotifyArtistsActionInitialized());
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