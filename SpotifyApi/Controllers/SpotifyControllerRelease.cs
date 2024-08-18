using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerRelease(IControllerApiRelease controllerApiRelease, ISpotifyControllerArtist controllerArtist) : ISpotifyControllerRelease
{
	private readonly IControllerApiRelease _controllerApiRelease = controllerApiRelease;
	private readonly ISpotifyControllerArtist _controllerArtist = controllerArtist;


	public async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>> GetReleasesFromArtist(ReleaseType releaseType, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>? existingArtists = null, bool forceUpdate = false)
	{
		// TODO editable
		var artists = await _controllerArtist.GetUserFollowedArtists(existingArtists, forceUpdate);
		if (artists is null)
		{
			// 0 playlists
			throw new NullReferenceException(nameof(artists));
		}
		if (artists.List is null)
		{
			// 0 playlists
			return artists;
		}

		if (artists.Update is null)
		{
			throw new NullReferenceException(nameof(artists.Update));
		}

		var lastUpdate = GetLastTimeUpdate(artists.Update, releaseType);
		var dateTimeDifference = DateTime.Now - lastUpdate;

		if (dateTimeDifference.TotalHours >= 24)
		{
			// force update every 24 hours
			forceUpdate = true;
		}


		// TODO force update - this will replace all existing releases from another release type - fixed down - need check
		/*if (!forceUpdate)
		{
			var existingArtistWithReleases = artists.List!.Any(x => x.Releases is not null && x.Releases.Any(y => y.ReleaseType == releaseType));
			if (!existingArtistWithReleases)
			{
				forceUpdate = true;
			}
		}*/

		if (!forceUpdate)
		{
			var existingArtistWithReleases = artists.List!.Any(x => x.Releases is not null && x.Releases.Any(y => y.ReleaseType == releaseType));

			if (existingArtistWithReleases)
			{
				// doesnt need update
				return artists;
			}
		}

		var artistsWithReleases = await GetReleasesFromArtistApi(artists.List, forceUpdate, artists.Update, releaseType);
		return artistsWithReleases;
	}

	private async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>> GetReleasesFromArtistApi(ISet<SpotifyArtist> artists, bool forceUpdate, SpotifyUserListUpdateArtists lastUpdateList, ReleaseType releaseType)
	{
		if (releaseType == ReleaseType.Podcasts)
		{
			return await GetPodcastReleasesFromArtistApi(artists, forceUpdate, lastUpdateList);
		}

		return await GetClassicReleasesFromArtistApi(artists, forceUpdate, lastUpdateList, releaseType);
	}

	private async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>> GetPodcastReleasesFromArtistApi(ISet<SpotifyArtist> artists, bool forceUpdate, SpotifyUserListUpdateArtists lastUpdateList)
	{
		// TODO podcasts
		throw new NotImplementedException(nameof(GetPodcastReleasesFromArtistApi));
	}

	private async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>> GetClassicReleasesFromArtistApi(ISet<SpotifyArtist> artistsSaved, bool forceUpdate, SpotifyUserListUpdateArtists lastUpdateList, ReleaseType releaseType)
	{
		if (releaseType == ReleaseType.Podcasts)
		{
			throw new NotSupportedException(nameof(releaseType));
		}

		var releases = await _controllerApiRelease.GetArtistsReleasesFromApi(artistsSaved, forceUpdate, releaseType);

		SetLastTimeUpdate(lastUpdateList, releaseType);

		var artistStorage = new SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>(releases, lastUpdateList);
		return artistStorage;
	}

	private DateTime GetLastTimeUpdate(SpotifyUserListUpdateArtists lastUpdateList, ReleaseType releaseType)
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

	private void SetLastTimeUpdate(SpotifyUserListUpdateArtists lastUpdateList, ReleaseType releaseType)
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






	/*private async Task<SpotifyUserList<SpotifyArtist><SpotifyRelease>> GetAllReleasesFromUserFollowedArtistsOld(ReleaseType releaseType, SpotifyUserList<SpotifyArtist><SpotifyRelease>? existingReleases = null, bool forceUpdate = false)
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
			var artistReleases = await GetArtistReleasesOld(artist, releaseType);
			releases.UnionWith(artistReleases);
		}

		var releasesList = new SpotifyUserList<SpotifyArtist><SpotifyRelease>(releases, DateTime.Now, releaseType);
		return releasesList;
	}


	private async Task<ISet<SpotifyRelease>> GetArtistReleasesOld(SpotifyArtist artist, ReleaseType releaseType)
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
	}*/
}