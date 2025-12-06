using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyArtistService
{
	Task<IReadOnlyList<SpotifyArtist>?> GetAll();
	Task Save(IReadOnlyList<SpotifyArtist> artists);
}