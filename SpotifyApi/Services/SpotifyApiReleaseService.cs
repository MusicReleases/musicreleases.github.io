using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.Services.Api;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Services;

internal class SpotifyApiReleaseService(IApiReleaseService controllerApiRelease, ISpotifyApiUserService controllerUser) : ISpotifyApiReleaseService
{
	private readonly IApiReleaseService _controllerApiRelease = controllerApiRelease;
	private readonly ISpotifyApiUserService _controllerUser = controllerUser;

	public async Task<SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>?> GetReleases(MainReleasesType releaseType, ISet<SpotifyArtist> artists, SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>? existingReleases = null, bool forceUpdate = false)
	{
		// doesnt force provide artists update!!!
		if (existingReleases is null)
		{
			forceUpdate = true;
		}
		else
		{
			var lastUpdate = ISpotifyApiReleaseService.GetLastTimeUpdate(existingReleases.Update, releaseType);
			if (lastUpdate.HasValue)
			{
				var dateTimeDifference = DateTime.Now - lastUpdate;

				if (dateTimeDifference.Value.TotalHours >= 24)
				{
					// force update every 24 hours
					forceUpdate = true;
				}
			}
			else
			{
				forceUpdate = true;
			}
		}

		if (!forceUpdate)
		{
			// doesnt need update
			return null;
		}

		var releases = await GetReleasesFromArtistApi(artists, existingReleases, forceUpdate, releaseType);
		return releases;
	}
	private async Task<SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>> GetReleasesFromArtistApi(ISet<SpotifyArtist> artists, SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>? existingReleases, bool forceUpdate, MainReleasesType releaseType)
	{
		if (releaseType == MainReleasesType.Podcasts)
		{
			return await GetPodcastReleasesFromArtistApi(artists, existingReleases, forceUpdate);
		}

		return await GetClassicReleasesFromArtistApi(artists, existingReleases, forceUpdate, releaseType);
	}

	private async Task<SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>> GetPodcastReleasesFromArtistApi(ISet<SpotifyArtist> artists, SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>? existingReleases, bool forceUpdate)
	{
		// TODO podcasts
		throw new NotImplementedException(nameof(GetPodcastReleasesFromArtistApi));
	}

	private async Task<SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>> GetClassicReleasesFromArtistApi(ISet<SpotifyArtist> artists, SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>? existingReleases, bool forceUpdate, MainReleasesType releaseType)
	{
		if (releaseType == MainReleasesType.Podcasts)
		{
			throw new NotSupportedException(nameof(releaseType));
		}

		SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>? releases;
		var releasesList = new HashSet<SpotifyReleaseOld>();
		var user = _controllerUser.GetUserRequired();
		var createNew = existingReleases?.List is null || existingReleases.Update is null;

		if (createNew || forceUpdate)
		{
			foreach (var artist in artists)
			{
				var releasesApi = await _controllerApiRelease.GetArtistReleasesFromApi(artist, releaseType);
				releasesList.UnionWith(releasesApi);
			}
			if (createNew)
			{
				// create new instance of releases
				var update = new SpotifyUserListUpdateRelease(DateTime.Now, releaseType);
				releases = new(releasesList, update);
			}
			else
			{
				// existing release list - append new and set last time update times
				releases = existingReleases;
				releases!.List!.UnionWith(releasesList);
				SetLastTimeUpdate(releases.Update!, releaseType);
			}
		}
		else
		{
			releases = existingReleases;
		}

		user.FollowedArtistReleases = releases;

		return releases!;
	}

	private void SetLastTimeUpdate(SpotifyUserListUpdateRelease lastUpdateList, MainReleasesType releaseType)
	{
		var lastUpdate = DateTime.Now;
		// update last update
		switch (releaseType)
		{
			case MainReleasesType.Albums:
				lastUpdateList.LastUpdateAlbums = lastUpdate;
				break;
			case MainReleasesType.Tracks:
				lastUpdateList.LastUpdateTracks = lastUpdate;
				break;
			case MainReleasesType.Appears:
				lastUpdateList.LastUpdateAppears = lastUpdate;
				break;
			case MainReleasesType.Compilations:
				lastUpdateList.LastUpdateCompilations = lastUpdate;
				break;
			case MainReleasesType.Podcasts:
				lastUpdateList.LastUpdatePodcasts = lastUpdate;
				break;
			default:
				throw new NotSupportedException(nameof(releaseType));
		}
	}
}