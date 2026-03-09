using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public interface IDbSpotifyService
{
	ValueTask<SpotifyDb> GetDb();
}