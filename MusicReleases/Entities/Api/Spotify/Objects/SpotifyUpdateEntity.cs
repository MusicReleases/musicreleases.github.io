using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;

public class SpotifyUpdateEntity<T> where T : SpotifyEntity
{
	public T Entity { get; set; }
	public DateTime Update { get; set; }

	public SpotifyUpdateEntity(T entity, DateTime update)
	{
		Entity = entity;
		Update = update;
	}
}
