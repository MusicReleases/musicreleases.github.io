using JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;
using Microsoft.Extensions.Primitives;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.BaseControllers;
public class LoginController(ISpotifyLoginController spotifyLoginController, IServiceTypeController serviceTypeController) : ILoginController
{
	private readonly ISpotifyLoginController _spotifyLoginController = spotifyLoginController;
	private readonly IServiceTypeController _serviceTypeController = serviceTypeController;

	public async Task LoginUser(ServiceType serviceType)
	{
		_serviceTypeController.Set(serviceType);

		switch (serviceType)
		{
			case ServiceType.Spotify:
				await _spotifyLoginController.LoginUser();
				break;
			default:
				throw new Exception("Unsupported service.");
		}
	}

	public async Task AutoLoginUser(ServiceType serviceType)
	{
		var savedUser = await IsUserSaved(serviceType);
		if (!savedUser)
		{
			return;
		}

		_serviceTypeController.Set(serviceType);
		await LoginUser(serviceType);
	}

	public async Task SetUser(ServiceType serviceType, StringValues code)
	{
		_serviceTypeController.Set(serviceType);
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
