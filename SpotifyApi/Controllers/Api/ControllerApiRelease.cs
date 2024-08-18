using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiRelease(ISpotifyApiClient client) : IControllerApiRelease
{
	private readonly ISpotifyApiClient _client = client;

	public async Task<ISet<SpotifyRelease>> GetArtistReleasesFromApi(string artistId, ReleaseType releaseType)
	{
		// TODO podcasts

		var albums = new SortedSet<SpotifyRelease>();
		var releasesFromApi = await GetArtistReleasesApi(artistId, releaseType);

		if (releasesFromApi == null)
		{
			return albums;
		}

		foreach (var releaseApi in releasesFromApi)
		{
			var album = new SpotifyRelease(releaseApi, releaseType);
			albums.Add(album);
		}

		return albums;
	}


	// artist releases tracks
	public async Task<ISet<SpotifyArtist>> GetArtistsReleasesFromApi(ISet<SpotifyArtist> artists, bool forceUpdate, ReleaseType releaseType)
	{
		foreach (var artist in artists)
		{
			var newReleases = await GetArtistReleases(artist, forceUpdate, releaseType) ?? [];
			if (forceUpdate)
			{
				artist.Releases = newReleases;
			}
			else
			{
				if (artist.Releases is null)
				{
					artist.Releases = [];
				}
				artist.Releases.UnionWith(newReleases);
			}
		}
		return artists;
	}

	private async Task<SortedSet<SpotifyRelease>?> GetArtistReleases(SpotifyArtist artist, bool forceUpdate, ReleaseType releaseType)
	{
		if (artist.Releases is not null && artist.Releases.Any(x => x.ReleaseType == releaseType))
		{
			// TODO force update (before)
			// doesnt need update
			return artist.Releases;
		}

		var releasesApi = await GetArtistReleasesApi(artist.Id, releaseType);
		if (releasesApi is null)
		{
			// 0 releases
			return [];
		}

		var releases = releasesApi.Select(x => new SpotifyRelease(x, releaseType));
		return new(releases);
	}

	private async Task<IList<SimpleAlbum>?> GetArtistReleasesApi(string artistId, ReleaseType releaseType)
	{
		if (releaseType == ReleaseType.Podcasts)
		{
			// TODO podcasts
			throw new NotImplementedException();
			//var request = new EpisodesRequest
		}

		var request = new ArtistsAlbumsRequest
		{
			Limit = ApiRequestLimit.ArtistReleases,
			IncludeGroupsParam = GetApiReleaseType(releaseType),
			// TODO market
		};
		// TODO all??
		/*if (releaseType != ReleaseType.All)
		{
			request.IncludeGroupsParam = GetApiReleaseType(releaseType);
		}*/

		var spotifyClient = _client.GetClient();

		try
		{
			var response = await spotifyClient.Artists.GetAlbums(artistId, request);
			var releases = await spotifyClient.PaginateAll(response);
			return releases;
		}
		catch (APIException e)
		{
			// TODO try again
			// Prints: invalid id
			Console.WriteLine(e.Message);
			// Prints: BadRequest
			Console.WriteLine(e.Response?.StatusCode); // 503 = service unavailable
			await Task.Delay(1000);
		}

		// TODO null
		return null;
	}
}