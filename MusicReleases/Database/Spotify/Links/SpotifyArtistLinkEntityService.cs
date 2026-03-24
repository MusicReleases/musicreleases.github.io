using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Database.Spotify.LinkEntities;

namespace JakubKastner.MusicReleases.Database.Spotify.Links;

internal abstract class SpotifyArtistLinkEntityService<TArtistLinkEntity> : SpotifyLinkEntityServiceCore<TArtistLinkEntity>
	where TArtistLinkEntity : class, ISpotifyDb, ISpotifyArtistLinkEntity
{
	protected readonly Dictionary<string, SpotifyArtistGroupByReleasePayload> _cache = [];

	public async Task Save(IReadOnlyCollection<TArtistLinkEntity> entities, CancellationToken ct)
	{
		await SaveEntities(entities, ct);

		foreach (var group in entities.GroupBy(x => x.ReleaseId))
		{
			_cache[group.Key] = group.ToPayload();
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