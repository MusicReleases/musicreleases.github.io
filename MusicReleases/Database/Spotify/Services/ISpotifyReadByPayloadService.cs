namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public interface ISpotifyReadByPayloadService<TModel, TPayload>
{
	Task<IReadOnlyCollection<TModel>> GetByIds(IReadOnlyCollection<TPayload> payloads, CancellationToken ct);
}