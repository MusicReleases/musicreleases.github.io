using JakubKastner.MusicReleases.Services.ApiServices;
using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Services.BaseServices;
public class LoginService(IApiLoginService apiLoginService) : ILoginService
{
	private readonly IApiLoginService _apiLoginService = apiLoginService;

	public async Task LoginUser()
	{
		await _apiLoginService.LoginUser();
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
		await _apiLoginService.SetUser(code);
	}

	public async Task<bool> IsUserSaved()
	{
		return await _apiLoginService.IsUserSaved();
	}

	public async Task LogoutUser()
	{
		// TODO stop all running api calls
		await _apiLoginService.LogoutUser();
	}
}
