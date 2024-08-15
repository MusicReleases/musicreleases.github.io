using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerRelease(IControllerApiRelease controllerApiRelease, ISpotifyControllerArtist controllerArtist) : ISpotifyControllerRelease
{
	private readonly IControllerApiRelease _controllerApiRelease = controllerApiRelease;
	private readonly ISpotifyControllerArtist _controllerArtist = controllerArtist;

	// get all releases for user followed artists
	public async Task<ISet<SpotifyRelease>> GetAllReleasesFromUserFollowed(ReleaseType releaseType = ReleaseType.Albums)
	{
		if (releaseType == ReleaseType.Podcasts)
		{
			return await GetAllReleasesFromUserFollowedPodcasts();
		}

		return await GetAllReleasesFromUserFollowedArtists(releaseType);
	}

	private async Task<ISet<SpotifyRelease>> GetAllReleasesFromUserFollowedArtists(ReleaseType releaseType)
	{
		if (releaseType == ReleaseType.Podcasts)
		{
			throw new NotSupportedException(nameof(releaseType));
		}

		// TODO provide saved artists
		var artists = await _controllerArtist.GetUserFollowedArtists();
		SortedSet<SpotifyRelease> releases = [];

		foreach (var artist in artists.List)
		{
			var artistReleases = await GetArtistReleases(artist, releaseType);
			releases.UnionWith(artistReleases);
		}

		return releases;
	}

	private async Task<ISet<SpotifyRelease>> GetAllReleasesFromUserFollowedPodcasts()
	{
		// TODO podcasts
		throw new NotImplementedException(nameof(GetAllReleasesFromUserFollowedPodcasts));
	}

	private async Task<ISet<SpotifyRelease>> GetArtistReleases(SpotifyArtist artist, ReleaseType releaseType)
	{
		// get saved releases from artist
		if (artist.Releases is null || !artist.Releases.Any(x => x.ReleaseType == releaseType))
		{
			var releasesFromApi = await _controllerApiRelease.GetArtistReleasesFromApi(artist.Id!, releaseType);
			// save release to artist releases list
			artist.Releases ??= [];
			artist.Releases.UnionWith(releasesFromApi);
		}

		// TODO try again
		//releasesFromApi ??= await _controllerApiRelease.GetArtistReleasesFromApi(artist.Id!, releaseType);

		var artistReleases = artist.Releases.Where(x => x.ReleaseType == releaseType).ToHashSet();

		return artistReleases;
	}
}