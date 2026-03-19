using JakubKastner.MusicReleases.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal interface ISpotifyUserLinkEntityService<TPayload> : ISpotifyLinkEntityService where TPayload : ISpotifyPayload
{
	Task DeleteAllForUser(string userId);
	Task<IReadOnlyCollection<TPayload>> GetByUserId(string userId, CancellationToken ct);
	Task Save(IReadOnlyCollection<TPayload> items, string userId, CancellationToken ct);
	Task Save(TPayload item, string userId, CancellationToken ct);
	Task SaveByUserId(string userId, IEnumerable<TPayload> payloads, CancellationToken ct);
}