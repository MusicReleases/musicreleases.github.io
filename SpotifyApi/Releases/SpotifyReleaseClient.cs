using JakubKastner.SpotifyApi.Artists;
using JakubKastner.SpotifyApi.Store;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Releases;

internal sealed class SpotifyReleaseClient(ISpotifyClientStore client) : ISpotifyReleaseClient
{
	private readonly ISpotifyClientStore _client = client;

	public async Task<List<SpotifyRelease>> GetByArtists(IEnumerable<SpotifyArtist> artists, ReleaseGroup releaseType, CancellationToken ct = default)
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

	public async Task<List<SpotifyRelease>> GetByArtist(SpotifyArtist artist, ReleaseGroup releaseType, CancellationToken ct = default)
	{
		if (releaseType == ReleaseGroup.Podcasts)
		{
			// TODO api podcasts - get
			throw new NotImplementedException();
		}

		var request = new ArtistsAlbumsRequest
		{
			Limit = ApiRequestLimit.ArtistReleases,
			IncludeGroupsParam = EnumReleaseTypeExtensions.GetApiReleaseGroup(releaseType),
			Offset = 0,
			Market = "from_token",
		};
		// TODO release group - all
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
			if (releaseType == ReleaseGroup.Appears)
			{
				featuredArtists.Add(artist);
			}

			var release = releaseApi.ToObject(featuredArtists);
			releases.Add(release);
		}

		return releases;
	}

	public async Task<List<SpotifyRelease>> GetByArtist(SpotifyArtist artist, ReleaseGroup releaseGroup, DateTime cutoff, CancellationToken ct = default)
	{
		if (releaseGroup == ReleaseGroup.Podcasts)
		{
			throw new NotImplementedException();
		}

		var spotifyClient = _client.GetClient();

		var request = new ArtistsAlbumsRequest
		{
			Limit = ApiRequestLimit.ArtistReleases,
			IncludeGroupsParam = EnumReleaseTypeExtensions.GetApiReleaseGroup(releaseGroup),
			Offset = 0,
			Market = "from_token",
		};

		var releases = new List<SpotifyRelease>();

		while (true)
		{
			ct.ThrowIfCancellationRequested();

			var page = await spotifyClient.Artists.GetAlbums(artist.Id, request, ct);
			if (page.Items is null || page.Items.Count == 0)
			{
				break;
			}

			var dates = page.Items.Select(i => i.ReleaseDate.ToDateTimeNullable()).ToList();

			// check if it's sorted descending (newest first) - if not, fallback to safe stop condition
			var hasNull = dates.Any(d => d is null);
			var isSortedDesc = true;

			DateTime? prev = null;
			for (int i = 0; i < dates.Count; i++)
			{
				var d = dates[i];
				if (d is null)
				{
					continue;
				}

				if (prev is not null && d.Value > prev.Value)
				{
					isSortedDesc = false;
					break;
				}
				prev = d;
			}

			foreach (var api in page.Items)
			{
				ct.ThrowIfCancellationRequested();

				var releaseDate = api.ReleaseDate.ToDateTimeNullable();

				if (releaseDate is null || releaseDate.Value >= cutoff)
				{
					var featuredArtists = new HashSet<SpotifyArtist>();
					if (releaseGroup == ReleaseGroup.Appears)
					{
						featuredArtists.Add(artist);
					}
					releases.Add(api.ToObject(featuredArtists));
				}
				else
				{
					if (!hasNull && isSortedDesc)
					{
						break;
					}
				}
			}

			// stop condition: if the page is sorted and has no null dates, we can just check the oldest date on the page
			if (!hasNull && isSortedDesc)
			{
				var oldestOnPage = dates.Where(d => d is not null).Min()!.Value;
				if (oldestOnPage < cutoff)
					break;
			}
			else
			{
				// safe stop condition: if the page is not sorted or has null dates, we need to check all dates on the page

				var allOlder = true;
				foreach (var d in dates)
				{
					if (d is null || d.Value >= cutoff)
					{
						allOlder = false;
						break;
					}
				}
				if (allOlder)
				{
					break;
				}
			}

			request.Offset += request.Limit ?? 50;
		}

		return releases;
	}
}