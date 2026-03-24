namespace JakubKastner.MusicReleases.Database.Spotify.BaseServices;

public interface IReadByPayloadService<TModel, TPayload>
{
	Task<IReadOnlyCollection<TModel>> GetByIds(
		IReadOnlyCollection<TPayload> payloads,
		CancellationToken ct);
}