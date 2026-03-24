using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Database.Spotify.LinkEntities;
using JakubKastner.MusicReleases.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify.Links;

internal abstract class SpotifyUserLinkEntityService<TUserIdEntity, TPayload> : SpotifyLinkOneKeyEntityService<TUserIdEntity, TPayload>, ISpotifyUserLinkEntityService<TPayload>
	where TUserIdEntity : class, ISpotifyDb, ISpotifyUserIdLinkEntity
	where TPayload : ISpotifyPayload
{
	public async Task<IReadOnlyCollection<TPayload>> GetByUserId(string userId, CancellationToken ct)
	{
		return await GetByKey1(userId, ct);
	}

	public async Task SaveByUserId(string userId, IEnumerable<TPayload> payloads, CancellationToken ct)
	{
		await SaveByKey1(userId, payloads, ct);
	}

	public async Task Save(string userId, IReadOnlyCollection<TPayload> items, CancellationToken ct)
	{
		await SaveByKey1(userId, items, ct);
	}

	public async Task Save(string userId, TPayload item, CancellationToken ct)
	{
		await SaveByKey1(userId, item, ct);
	}

	public async Task DeleteAllForUser(string userId)
	{
		await DeleteAllByKey1(userId);
	}
}