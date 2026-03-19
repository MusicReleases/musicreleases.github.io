using JakubKastner.MusicReleases.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal interface ISpotifyUserLinkEntityService : ISpotifyLinkEntityService
{
	Task DeleteAllForUser(string userId);
}

internal interface ISpotifyUserLinkEntityService<TPayload> : ISpotifyUserLinkEntityService where TPayload : ISpotifyPayload
{
	Task<IReadOnlyCollection<TPayload>> GetByUserId(string userId, CancellationToken ct);
	Task Save(IReadOnlyCollection<TPayload> items, string userId, CancellationToken ct);
	Task Save(TPayload item, string userId, CancellationToken ct);
	Task SaveByUserId(string userId, IEnumerable<TPayload> payloads, CancellationToken ct);
}