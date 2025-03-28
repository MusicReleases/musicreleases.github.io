using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.Services.Api;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Services;

internal class SpotifyReleaseService(IApiReleaseService controllerApiRelease, ISpotifyArtistService controllerArtist, ISpotifyUserService controllerUser) : ISpotifyReleaseService
{
	private readonly IApiReleaseService _controllerApiRelease = controllerApiRelease;
	private readonly ISpotifyArtistService _controllerArtist = controllerArtist;
	private readonly ISpotifyUserService _controllerUser = controllerUser;

	public async Task<SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>> GetReleases(ReleaseType releaseType, ISet<SpotifyArtist> artists, SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? existingReleases = null, bool forceUpdate = false)
	{
		// doesnt force provide artists update!!!
		if (existingReleases is null)
		{
			forceUpdate = true;
		}
		else
		{
			var lastUpdate = GetLastTimeUpdate(existingReleases.Update, releaseType);
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
			return existingReleases!;
		}

		var releases = await GetReleasesFromArtistApi(artists, existingReleases, forceUpdate, releaseType);
		return releases;
	}
	private async Task<SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>> GetReleasesFromArtistApi(ISet<SpotifyArtist> artists, SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? existingReleases, bool forceUpdate, ReleaseType releaseType)
	{
		if (releaseType == ReleaseType.Podcasts)
		{
			return await GetPodcastReleasesFromArtistApi(artists, existingReleases, forceUpdate);
		}

		return await GetClassicReleasesFromArtistApi(artists, existingReleases, forceUpdate, releaseType);
	}

	private async Task<SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>> GetPodcastReleasesFromArtistApi(ISet<SpotifyArtist> artists, SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? existingReleases, bool forceUpdate)
	{
		// TODO podcasts
		throw new NotImplementedException(nameof(GetPodcastReleasesFromArtistApi));
	}

	private async Task<SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>> GetClassicReleasesFromArtistApi(ISet<SpotifyArtist> artists, SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? existingReleases, bool forceUpdate, ReleaseType releaseType)
	{
		if (releaseType == ReleaseType.Podcasts)
		{
			throw new NotSupportedException(nameof(releaseType));
		}

		SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? releases;
		var releasesList = new HashSet<SpotifyRelease>();
		var user = _controllerUser.GetUserRequired();
		var createNew = existingReleases?.List is null || existingReleases.Update is null;

		if (createNew || forceUpdate)
		{
			foreach (var artist in artists)
			{
				var releasesApi = await _controllerApiRelease.GetArtistReleasesFromApi(artist.Id, releaseType);
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

	private DateTime? GetLastTimeUpdate(SpotifyUserListUpdateRelease lastUpdateList, ReleaseType releaseType)
	{
		return releaseType switch
		{
			ReleaseType.Albums => lastUpdateList.LastUpdateAlbums,
			ReleaseType.Tracks => lastUpdateList.LastUpdateTracks,
			ReleaseType.Appears => lastUpdateList.LastUpdateAppears,
			ReleaseType.Compilations => lastUpdateList.LastUpdateCompilations,
			ReleaseType.Podcasts => lastUpdateList.LastUpdatePodcasts,
			_ => throw new NotSupportedException(nameof(releaseType)),
		};
	}
	private void SetLastTimeUpdate(SpotifyUserListUpdateRelease lastUpdateList, ReleaseType releaseType)
	{
		var lastUpdate = DateTime.Now;
		// update last update
		switch (releaseType)
		{
			case ReleaseType.Albums:
				lastUpdateList.LastUpdateAlbums = lastUpdate;
				break;
			case ReleaseType.Tracks:
				lastUpdateList.LastUpdateTracks = lastUpdate;
				break;
			case ReleaseType.Appears:
				lastUpdateList.LastUpdateAppears = lastUpdate;
				break;
			case ReleaseType.Compilations:
				lastUpdateList.LastUpdateCompilations = lastUpdate;
				break;
			case ReleaseType.Podcasts:
				lastUpdateList.LastUpdatePodcasts = lastUpdate;
				break;
			default:
				throw new NotSupportedException(nameof(releaseType));
		}
	}
}