using JakubKastner.MusicReleases.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify.Links;

internal interface ISpotifyUserLinkEntityService
{
	Task DeleteAllForUser(string userId);
}

internal interface ISpotifyUserLinkEntityService<TPayload> where TPayload : ISpotifyPayload
{
	Task<IReadOnlyCollection<TPayload>> GetByUserId(string userId, CancellationToken ct);
	Task Save(string userId, IReadOnlyCollection<TPayload> items, CancellationToken ct);
	Task Save(string userId, TPayload item, CancellationToken ct);
	Task SaveByUserId(string userId, IEnumerable<TPayload> payloads, CancellationToken ct);
}