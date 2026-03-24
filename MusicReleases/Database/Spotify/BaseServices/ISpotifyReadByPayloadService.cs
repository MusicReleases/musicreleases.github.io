namespace JakubKastner.MusicReleases.Database.Spotify.BaseServices;

public interface ISpotifyReadByPayloadService<TModel, TPayload>
{
	Task<IReadOnlyCollection<TModel>> GetByIds(IReadOnlyCollection<TPayload> payloads, CancellationToken ct);
}