using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Database.Spotify.LinkEntities;
using JakubKastner.MusicReleases.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify.Links;

internal abstract class SpotifyUserLinkEntityService<TUserIdEntity, TPayload> : SpotifyLinkOneKeyEntityService<TUserIdEntity, TPayload>, ISpotifyUserLinkEntityService<TPayload>
	where TUserIdEntity : ISpotifyDb, ISpotifyUserIdLinkEntity
	where TPayload : ISpotifyPayload
{
	public async Task<IReadOnlyCollection<TPayload>> GetByUserId(string userId, CancellationToken ct) => await GetByKey1(userId, ct);

	public async Task SaveByUserId(string userId, IEnumerable<TPayload> payloads, CancellationToken ct) => await SaveByKey1(userId, payloads, ct);

	public async Task Save(IReadOnlyCollection<TPayload> items, string userId, CancellationToken ct) => await SaveByKey1(items, userId, ct);

	public async Task Save(TPayload item, string userId, CancellationToken ct) => await SaveByKey1(item, userId, ct);

	public async Task DeleteAllForUser(string userId) => await DeleteAllByKey1(userId);
}