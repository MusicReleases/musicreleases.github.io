using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Spotify.Artists.User;
using JakubKastner.SpotifyApi.Artists;

namespace JakubKastner.MusicReleases.Spotify.Playlists.User;

public static class SpotifyUserArtistMapper
{
	public static SpotifyUserArtistEntity ToEntity(this SpotifyUserArtistPayload payload, string userId)
	{
		return new(userId, payload.Id);
	}

	public static SpotifyUserArtistPayload ToPayload(this SpotifyUserArtistEntity entity)
	{
		return new(entity.ArtistId);
	}

	public static SpotifyUserArtistPayload ToPayload(this SpotifyArtist model)
	{
		return new(model.Id);
	}
}