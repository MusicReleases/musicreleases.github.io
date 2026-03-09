using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Mappers;

public static class SpotifyReleaseMapper
{
	public static SpotifyReleaseEntity ToEntity(this SpotifyRelease dto)
	{
		return new(dto.Id, dto.Name, dto.ReleaseType, dto.ReleaseDate, dto.UrlApp, dto.UrlWeb, dto.UrlImage, dto.TotalTracks);
	}

	public static SpotifyRelease ToModel(this SpotifyReleaseEntity entity, HashSet<SpotifyArtist> artists, HashSet<SpotifyArtist> featuredArtists)
	{
		return new(entity.Id, entity.Name, entity.ReleaseType, entity.ReleaseDate, entity.UrlApp, entity.UrlWeb, entity.UrlImage, entity.TotalTracks, artists, featuredArtists);
	}
}
