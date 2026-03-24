using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Database.Spotify;

public interface IDbSpotifyService
{
	ValueTask<SpotifyDb> GetDb();
}