using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.SpotifyApi.Objects.Base;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Database.Spotify.IdEntities;

internal interface ISpotifyIdEntityStoreService<TModel>
	where TModel : SpotifyIdNameObject
{
	Task Save(IReadOnlyCollection<TModel> items, bool keepExisting, CancellationToken ct);
	Task Save(TModel item, bool keepExisting, CancellationToken ct);
}

internal interface ISpotifyIdEntityStoreService<TModel, TIdEntity> : ISpotifyIdEntityStoreService<TModel>
	where TModel : SpotifyIdNameObject
	where TIdEntity : ISpotifyDb, ISpotifyIdEntity
{
	Task Update(string id, Expression<Func<TIdEntity, string>> query, string newValue, CancellationToken ct);
}