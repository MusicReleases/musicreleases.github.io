using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Mappers;

public static class SpotifyArtistMapper
{
	public static SpotifyArtistEntity ToEntity(this SpotifyArtist dto)
	{
		return new(dto.Id, dto.Name, dto.UrlApp, dto.UrlWeb);
	}

	public static SpotifyArtist ToModel(this SpotifyArtistEntity entity)
	{
		return new(entity.Id, entity.Name, entity.UrlApp, entity.UrlWeb);
	}
}
