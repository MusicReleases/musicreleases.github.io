using Microsoft.Extensions.Primitives;
using static JakubKastner.MusicReleases.Base.Enums;

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
