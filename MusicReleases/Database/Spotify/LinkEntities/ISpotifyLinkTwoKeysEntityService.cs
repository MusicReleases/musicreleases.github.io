using JakubKastner.MusicReleases.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify.LinkEntities;

internal interface ISpotifyLinkTwoKeysEntityService<TPayload2> : ISpotifyLinkOneKeyEntityService
	where TPayload2 : ISpotifyPayload
{
	Task DeleteAllByKey2(string key2);
	Task SaveByKey2(IReadOnlyCollection<TPayload2> items, string key2, CancellationToken ct);
	Task SaveByKey2(string key2, IEnumerable<TPayload2> payloads, CancellationToken ct);
	Task SaveByKey2(TPayload2 item, string key2, CancellationToken ct);
}