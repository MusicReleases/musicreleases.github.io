using JakubKastner.MusicReleases.Controllers.Spotify;
using Microsoft.Extensions.Primitives;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.Base;
public class LoginController : ILoginController
{
	private readonly ISpotifyLoginController _spotifyLoginController;

	public LoginController(ISpotifyLoginController spotifyLoginController)
	{
		_spotifyLoginController = spotifyLoginController;
	}

	public async Task LoginUser(ServiceType type)
	{
		switch (type)
		{
			case ServiceType.Spotify:
				await _spotifyLoginController.LoginUser();
				break;
			default:
				throw new Exception("Unsupported service.");
		}
	}

	public async Task SetUser(ServiceType type, StringValues code)
	{
		switch (type)
		{
			case ServiceType.Spotify:
				await _spotifyLoginController.SetUser(code);
				break;
			default:
				throw new Exception("Unsupported service.");
		}
	}

	public async Task<bool> IsUserSaved(ServiceType type)
	{
		return type switch
		{
			ServiceType.Spotify => await _spotifyLoginController.IsUserSaved(),
			_ => throw new Exception("Unsupported service."),
		};
	}
}
