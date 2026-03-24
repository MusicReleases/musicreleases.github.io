namespace JakubKastner.MusicReleases.Database.Spotify.BaseServices;

public interface IWriteEntityService<TModel>
{
	Task Save(IReadOnlyCollection<TModel> models, bool keepExisting, CancellationToken ct);

	Task Save(TModel model, bool keepExisting, CancellationToken ct);
}