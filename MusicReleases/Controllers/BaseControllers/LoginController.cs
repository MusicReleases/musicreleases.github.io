using JakubKastner.MusicReleases.Controllers.ApiControllers;
using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Controllers.BaseControllers;
public class LoginController(IBaseLoginController baseLoginController) : ILoginController
{
	private readonly IBaseLoginController _baseLoginController = baseLoginController;

	public async Task LoginUser()
	{
		await _baseLoginController.LoginUser();
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
		await _baseLoginController.SetUser(code);
	}

	public async Task<bool> IsUserSaved()
	{
		return await _baseLoginController.IsUserSaved();
	}

	public async Task LogoutUser()
	{
		// TODO stop all running api calls
		await _baseLoginController.LogoutUser();
	}
}
