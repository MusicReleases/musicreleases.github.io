using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.SpotifyApi.Controllers;

public class ControllerTrack
{
    private readonly ControllerApiRelease _controllerApiRelease;
    private readonly ControllerArtist _controllerArtist;

    public ControllerTrack(ControllerApiRelease controllerApiRelease, ControllerArtist controllerArtist)
    {
        _controllerApiRelease = controllerApiRelease;
        _controllerArtist = controllerArtist;
    }

    // get all relases for user followed artist
    public async Task<SortedSet<Album>> GetAllUserFollowedArtistsReleases(ReleaseType releaseType = ReleaseType.Albums)
    {
        var artists = await _controllerArtist.GetUserFollowedArtists();
        SortedSet<Album> releases = new();

        foreach (var artist in artists)
        {
            // TODO save release to artist releases list
            var releasesFromApi = await _controllerApiRelease.GetArtistReleasesFromApi(artist.Id, releaseType);
            // TODO try again
            releasesFromApi ??= await _controllerApiRelease.GetArtistReleasesFromApi(artist.Id, releaseType);
            releases.UnionWith(releasesFromApi);
        }

        return releases;
    }
}