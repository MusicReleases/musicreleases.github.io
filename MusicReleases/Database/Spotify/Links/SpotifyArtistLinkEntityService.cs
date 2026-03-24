using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Database.Spotify.LinkEntities;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Database.Spotify.Links;

internal abstract class SpotifyArtistLinkEntityService<TArtistLinkEntity>
	: SpotifyLinkEntityServiceCore<TArtistLinkEntity>
	where TArtistLinkEntity : class, ISpotifyDb, ISpotifyArtistLinkEntity
{
	protected readonly Dictionary<string, SpotifyArtistGroupByReleasePayload> _cache = [];

	public async Task Save(IReadOnlyCollection<TArtistLinkEntity> entities, CancellationToken ct)
	{
		await SaveEntities(entities, ct);

		foreach (var group in entities.GroupBy(x => x.ReleaseId))
		{
			HashSet<string> mainArtistIds = new();
			HashSet<string> featuredArtistIds = new();

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

			_cache[group.Key] = new SpotifyArtistGroupByReleasePayload(
				group.Key,
				mainArtistIds,
				featuredArtistIds
			);
		}
	}

	protected void InvalidateRelease(string releaseId)
	{
		_cache.Remove(releaseId);
	}

	protected void ClearArtistCache()
	{
		_cache.Clear();
	}
}