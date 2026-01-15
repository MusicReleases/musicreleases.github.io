using JakubKastner.MusicReleases.Enums;
using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Services.ApiServices;

public interface IApiLoginService
{
	Task<bool> IsUserSaved();
	bool IsUserLoggedIn();
	Task LoginUser();
	Task LogoutUser();
	Task SetUser(StringValues code);

	ServiceType GetServiceType();
}
