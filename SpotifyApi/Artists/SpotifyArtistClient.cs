using JakubKastner.SpotifyApi.Store;
using SpotifyAPI.Web;
using System.Runtime.CompilerServices;

namespace JakubKastner.SpotifyApi.Artists;

internal sealed class SpotifyArtistClient(ISpotifyClientStore client) : ISpotifyArtistClient
{
	private readonly ISpotifyClientStore _client = client;

	public async Task<IReadOnlyCollection<SpotifyArtist>> GetFollowed(CancellationToken ct = default)
	{
		var request = new FollowOfCurrentUserRequest(FollowOfCurrentUserRequest.Type.Artist)
		{
			Limit = ApiRequestLimit.UserFollowedArtists,
		};

		var spotifyClient = _client.GetClient();
		var response = await spotifyClient.Follow.OfCurrentUser(request, ct);
		var artistsAsync = spotifyClient.Paginate(response.Artists, s => s.Artists, cancel: ct);

		var artists = new List<SpotifyArtist>();
		await foreach (var artistApi in artistsAsync.WithCancellation(ct))
		{
			var artist = artistApi.ToObject();
			artists.Add(artist);
		}

		return artists.AsReadOnly();
	}

	public async IAsyncEnumerable<IReadOnlyCollection<SpotifyArtist>> GetFollowedBatches(int batchSize = 25, [EnumeratorCancellation] CancellationToken ct = default)
	{
		var request = new FollowOfCurrentUserRequest(FollowOfCurrentUserRequest.Type.Artist)
		{
			Limit = ApiRequestLimit.UserFollowedArtists,
		};

		var spotifyClient = _client.GetClient();
		var response = await spotifyClient.Follow.OfCurrentUser(request, ct);

		var paged = spotifyClient.Paginate(response.Artists, s => s.Artists, cancel: ct);

		var batch = new List<SpotifyArtist>(batchSize);

		await foreach (var artistApi in paged.WithCancellation(ct))
		{
			ct.ThrowIfCancellationRequested();

			batch.Add(artistApi.ToObject());

			if (batch.Count == batchSize)
			{
				yield return batch.ToArray();
				batch.Clear();

				await Task.Yield();
			}
		}

		if (batch.Count > 0)
		{
			yield return batch.ToArray();
		}
	}
}