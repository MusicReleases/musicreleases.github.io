using JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;
using Microsoft.Extensions.Primitives;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.BaseControllers;
public class LoginController : ILoginController
{
    private readonly ISpotifyLoginController _spotifyLoginController;
    private readonly IServiceTypeController _serviceTypeController;


    public LoginController(ISpotifyLoginController spotifyLoginController, IServiceTypeController serviceTypeController)
    {
        _spotifyLoginController = spotifyLoginController;
        _serviceTypeController = serviceTypeController;
    }

    public async Task LoginUser(ServiceType serviceType)
    {
        switch (serviceType)
        {
            case ServiceType.Spotify:
                await _spotifyLoginController.LoginUser();
                break;
            default:
                throw new Exception("Unsupported service.");
        }

        _serviceTypeController.Set(serviceType);
    }

    public async Task AutoLoginUser(ServiceType serviceType)
    {
        var savedUser = await IsUserSaved(serviceType);
        if (!savedUser)
        {
            return;
        }

        await LoginUser(serviceType);
    }

    public async Task SetUser(ServiceType type, StringValues code)
    {
        var serviceType = _serviceTypeController.GetRequired();

        switch (serviceType)
        {
            case ServiceType.Spotify:
                await _spotifyLoginController.SetUser(code);
                break;
            default:
                throw new Exception("Unsupported service.");
        }
    }

    public async Task<bool> IsUserSaved(ServiceType serviceType)
    {
        return serviceType switch
        {
            ServiceType.Spotify => await _spotifyLoginController.IsUserSaved(),
            _ => throw new Exception("Unsupported service."),
        };
    }
}
