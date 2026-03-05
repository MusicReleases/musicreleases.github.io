using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Services.Api;

internal class ApiReleaseServiceOld(ISpotifyApiClient client) : IApiReleaseService
{
	private readonly ISpotifyApiClient _client = client;

	public async Task<ISet<SpotifyReleaseOld>> GetArtistReleasesFromApi(SpotifyArtist artist, MainReleasesType releaseType)
	{
		// TODO podcasts

		var albums = new SortedSet<SpotifyReleaseOld>();
		var releasesFromApi = await GetArtistReleasesApi(artist.Id, releaseType);

		if (releasesFromApi == null)
		{
			return albums;
		}

		foreach (var releaseApi in releasesFromApi)
		{
			var album = new SpotifyReleaseOld(releaseApi, releaseType);

			if (releaseType == MainReleasesType.Appears)
			{
				// add current artist to album on appears
				album.Artists.Add(artist);
			}
			albums.Add(album);
		}

		return albums;
	}

	private async Task<IList<SimpleAlbum>?> GetArtistReleasesApi(string artistId, MainReleasesType releaseType)
	{
		if (releaseType == MainReleasesType.Podcasts)
		{
			// TODO podcasts
			throw new NotImplementedException();
			//var request = new EpisodesRequest
		}

		var request = new ArtistsAlbumsRequest
		{
			Limit = ApiRequestLimit.ArtistReleases,
			IncludeGroupsParam = EnumReleaseTypeExtensions.GetApiReleaseType(releaseType),
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