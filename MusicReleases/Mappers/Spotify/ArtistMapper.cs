using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Mappers.Spotify;

public static class ArtistMapper
{
	public static SpotifyArtistEntity ToEntity(this SpotifyArtist dto)
	{
		return new SpotifyArtistEntity(dto.Id, dto.Name, dto.UrlApp, dto.UrlWeb);
	}

	public static SpotifyArtist ToModel(this SpotifyArtistEntity entity)
	{
		return new SpotifyArtist(entity.Id, entity.Name, entity.UrlApp, entity.UrlWeb);
	}
}
