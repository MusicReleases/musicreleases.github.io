using JakubKastner.SpotifyApi.Store;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Clients;

internal class SpotifyReleaseClient(ISpotifyClientStore client) : ISpotifyReleaseClient
{
	private readonly ISpotifyClientStore _client = client;

	public async Task<List<SpotifyRelease>> GetByArtists(IEnumerable<SpotifyArtist> artists, ReleaseEnums releaseType, CancellationToken ct = default)
	{
		var set = new HashSet<SpotifyRelease>();

		foreach (var artist in artists)
		{
			ct.ThrowIfCancellationRequested();
			var releases = await GetByArtist(artist, releaseType, ct);
			set.UnionWith(releases);
		}

		return [.. set];
	}

	public async Task<List<SpotifyRelease>> GetByArtist(SpotifyArtist artist, ReleaseEnums releaseType, CancellationToken ct = default)
	{
		if (releaseType == ReleaseEnums.Podcasts)
		{
			// TODO podcasts
			throw new NotImplementedException();
		}

		var request = new ArtistsAlbumsRequest
		{
			Limit = ApiRequestLimit.ArtistReleases,
			IncludeGroupsParam = EnumReleaseTypeExtensions.GetApiReleaseGroup(releaseType),
		};
		// TODO all??
		/*if (releaseType != ReleaseType.All)
		{
			request.IncludeGroupsParam = GetApiReleaseType(releaseType);
		}*/

		var spotifyClient = _client.GetClient();
		var response = await spotifyClient.Artists.GetAlbums(artist.Id, request, ct);
		var releasesAsync = spotifyClient.Paginate(response, cancel: ct);

		var releases = new List<SpotifyRelease>();
		await foreach (var releaseApi in releasesAsync.WithCancellation(ct))
		{
			var featuredArtists = new HashSet<SpotifyArtist>();
			if (releaseType == ReleaseEnums.Appears)
			{
				featuredArtists.Add(artist);
			}

			var release = releaseApi.ToObject(featuredArtists);
			releases.Add(release);
		}

		return releases;
	}
}