using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers
{
    public interface ISpotifyLoginController
    {
        Task<bool> IsUserSaved();
        Task LoginUser();
        Task SetUser(StringValues code);
    }
}