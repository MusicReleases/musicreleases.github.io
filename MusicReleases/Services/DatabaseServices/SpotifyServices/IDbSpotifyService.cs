using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyService
{
	ValueTask<SpotifyDb> GetDb();
}