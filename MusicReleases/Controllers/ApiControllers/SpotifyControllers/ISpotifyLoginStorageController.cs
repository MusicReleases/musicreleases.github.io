using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers
{
    public interface ISpotifyLoginStorageController
    {
        Task DeleteLoginVerifier();
        Task DeleteSavedUser();
        Task<string> GetLoginVerifier();
        Task<SpotifyUser?> GetSavedUser();
        Task SaveLoginVerifier(string codeVerifier);
        Task SaveUser();
    }
}