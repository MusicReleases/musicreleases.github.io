using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;

public class SpotifyUpdateDbObject<T> where T : ISpotifyIdEntity
{
	public T Entity { get; set; }
	public DateTime Update { get; set; }

	public SpotifyUpdateDbObject(T entity, DateTime update)
	{
		Entity = entity;
		Update = update;
	}
}

public class SpotifyUpdateDbObjectOld<T> where T : SpotifyIdEntity
{
	public T Entity { get; set; }
	public DateTime Update { get; set; }

	public SpotifyUpdateDbObjectOld(T entity, DateTime update)
	{
		Entity = entity;
		Update = update;
	}
}
