using JakubKastner.MusicReleases.Controllers.ApiControllers;
using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Controllers.BaseControllers;
public class LoginController(IApiLoginController apiLoginController) : ILoginController
{
	private readonly IApiLoginController _apiLoginController = apiLoginController;

	public async Task LoginUser()
	{
		await _apiLoginController.LoginUser();
	}

	public async Task AutoLoginUser()
	{
		var savedUser = await IsUserSaved();
		if (!savedUser)
		{
			return;
		}
		await LoginUser();
	}

	public async Task SetUser(StringValues code)
	{
		await _apiLoginController.SetUser(code);
	}

	public async Task<bool> IsUserSaved()
	{
		return await _apiLoginController.IsUserSaved();
	}

	public async Task LogoutUser()
	{
		// TODO stop all running api calls
		await _apiLoginController.LogoutUser();
	}
}
