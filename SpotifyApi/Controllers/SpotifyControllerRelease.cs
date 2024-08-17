using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerRelease(IControllerApiRelease controllerApiRelease, ISpotifyControllerArtist controllerArtist) : ISpotifyControllerRelease
{
	private readonly IControllerApiRelease _controllerApiRelease = controllerApiRelease;
	private readonly ISpotifyControllerArtist _controllerArtist = controllerArtist;

	// get all releases for user followed artists
	public async Task<SpotifyUserListReleases<SpotifyRelease>> GetAllReleasesFromUserFollowed(ReleaseType releaseType = ReleaseType.Albums, SpotifyUserListReleases<SpotifyRelease>? existingReleases = null, bool forceUpdate = false)
	{
		var otherType = false;

		if (existingReleases is null)
		{
			forceUpdate = true;
		}
		else if (!existingReleases.List!.Any(x => x.ReleaseType == releaseType))
		{
			// not found any reqired release type
			forceUpdate = true;
			otherType = true;
		}
		else
		{
			var dateTimeDifference = DateTime.Now - existingReleases.LastUpdateMain;

			if (dateTimeDifference.TotalHours >= 24)
			{
				// force update every 24 hours
				forceUpdate = true;
			}
		}

		if (!forceUpdate)
		{
			// doesnt need update

			// TODO editable playlists switch ???
			return existingReleases!;
		}

		var releases = await GetAllReleasesApi(releaseType, existingReleases, forceUpdate);
		if (otherType)
		{
			//releases
		}
		return releases;
	}

	public async Task<SpotifyUserListReleases<SpotifyRelease>> GetAllReleasesApi(ReleaseType releaseType = ReleaseType.Albums, SpotifyUserListReleases<SpotifyRelease>? existingReleases = null, bool forceUpdate = false)
	{
		if (releaseType == ReleaseType.Podcasts)
		{
			return await GetAllReleasesFromUserFollowedPodcasts(existingReleases, forceUpdate);
		}

		return await GetAllReleasesFromUserFollowedArtists(releaseType, existingReleases, forceUpdate);
	}

	private async Task<SpotifyUserListReleases<SpotifyRelease>> GetAllReleasesFromUserFollowedArtists(ReleaseType releaseType, SpotifyUserListReleases<SpotifyRelease>? existingReleases = null, bool forceUpdate = false)
	{
		if (releaseType == ReleaseType.Podcasts)
		{
			throw new NotSupportedException(nameof(releaseType));
		}

		// TODO provide saved artists && force update
		var artists = await _controllerArtist.GetUserFollowedArtists(null, false);

		if (artists.List is null)
		{
			throw new NullReferenceException(nameof(artists.List));
		}

		var releases = existingReleases?.List ?? new SortedSet<SpotifyRelease>();
		foreach (var artist in artists.List)
		{
			var artistReleases = await GetArtistReleases(artist, releaseType);
			releases.UnionWith(artistReleases);
		}

		var releasesList = new SpotifyUserListReleases<SpotifyRelease>(releases, DateTime.Now, releaseType);
		return releasesList;
	}

	private async Task<SpotifyUserListReleases<SpotifyRelease>> GetAllReleasesFromUserFollowedPodcasts(SpotifyUserListReleases<SpotifyRelease>? existingReleases = null, bool forceUpdate = false)
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