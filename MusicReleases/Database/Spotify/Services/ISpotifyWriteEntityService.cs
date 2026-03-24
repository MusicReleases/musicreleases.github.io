namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public interface ISpotifyWriteEntityService<TModel>
{
	Task Save(IReadOnlyCollection<TModel> models, bool keepExisting, CancellationToken ct);

	Task Save(TModel model, bool keepExisting, CancellationToken ct);
}