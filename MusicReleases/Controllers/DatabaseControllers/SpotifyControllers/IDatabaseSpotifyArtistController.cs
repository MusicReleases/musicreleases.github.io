using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers;

public interface IDatabaseSpotifyArtistController
{
	Task<ISet<SpotifyArtist>?> GetAll();
	Task Save(ISet<SpotifyArtist> artists);
}