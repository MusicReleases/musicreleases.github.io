using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Database.Spotify.IdEntities;

internal interface ISpotifyIdEntityStoreService<TModel>
	where TModel : SpotifyIdNameObject
{
	Task Save(IReadOnlyCollection<TModel> items, bool keepExisting, CancellationToken ct);
	Task Save(TModel item, bool keepExisting, CancellationToken ct);
}