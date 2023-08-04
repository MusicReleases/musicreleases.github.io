using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;
using static JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiRelease
{
	private readonly SpotifyClient _client;

	public ControllerApiRelease(SpotifyClient client)
	{
		_client = client;
	}

	public async Task<SortedSet<SpotifyAlbum>> GetArtistReleasesFromApi(string artistId, ReleaseType releaseType)
	{
		// TODO podcasts

		var albums = new SortedSet<SpotifyAlbum>();
		var releasesFromApi = await GetArtistReleasesApi(artistId, releaseType);

		if (releasesFromApi == null)
		{
			return albums;
		}

		foreach (var releaseApi in releasesFromApi)
		{
			var album = new SpotifyAlbum(releaseApi);
			albums.Add(album);
		}

		return albums;
	}

	private async Task<IList<SimpleAlbum>?> GetArtistReleasesApi(string artistId, ReleaseType releaseType)
	{
		if (releaseType == ReleaseType.Podcasts)
		{
			// TODO podcasts
			throw new Exception("TODO");
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