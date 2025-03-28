using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyArtistService
{
	Task<ISet<SpotifyArtist>?> GetAll();
	Task Save(ISet<SpotifyArtist> artists);
}