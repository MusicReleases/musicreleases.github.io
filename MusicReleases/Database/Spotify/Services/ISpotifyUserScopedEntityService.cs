namespace JakubKastner.MusicReleases.Database.Spotify.BaseServices;

internal interface ISpotifyUserScopedEntityService
{
	Task DeleteByUserId(string userId, CancellationToken ct);
	Task DeleteForCurrentUser(CancellationToken ct);

}
internal interface ISpotifyUserScopedEntityService<TModel> : ISpotifyUserScopedEntityService
	where TModel : class
{
	Task<TModel?> Get(CancellationToken ct);
	Task<TModel?> GetByUserId(string userId, CancellationToken ct);
	Task Save(TModel model, bool keepExisting, CancellationToken ct);
}