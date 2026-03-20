using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal abstract class SpotifyArtistLinkService<TArtistLinkEntity>
	: SpotifyLinkEntityServiceCore<TArtistLinkEntity>, ISpotifyArtistLinkEntityService<TArtistLinkEntity> where TArtistLinkEntity : ISpotifyDb, ISpotifyArtistLinkEntity
{
	protected readonly Dictionary<string, SpotifyArtistGroupByReleasePayload> _cache = [];

	/*protected override Expression<Func<TArtistLinkEntity, string>> Key1Expression => x => x.ArtistId;

	protected override Expression<Func<TArtistLinkEntity, string>> Key2Expression => x => x.ReleaseId;*/

	public async Task Save(IReadOnlyCollection<TArtistLinkEntity> entities, CancellationToken ct)
	{
		await SaveEntities(entities, ct);

		foreach (var group in entities.GroupBy(x => x.ReleaseId))
		{
			if (_cache.TryGetValue(group.Key, out var payload))
			{
				// saved
				continue;
			}

			HashSet<string> mainArtistIds = [];
			HashSet<string> featuredArtistIds = [];

			foreach (var entity in group)
			{
				if (entity.Role == ArtistReleaseRole.Main)
				{
					mainArtistIds.Add(entity.ArtistId);
				}
				else if (entity.Role == ArtistReleaseRole.Featured)
				{
					featuredArtistIds.Add(entity.ArtistId);
				}
			}

			_cache[group.Key] = new(group.Key, mainArtistIds, featuredArtistIds);
		}
	}
}