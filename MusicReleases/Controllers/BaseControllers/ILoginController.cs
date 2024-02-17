using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Controllers.BaseControllers;

public interface ILoginController
{
	Task AutoLoginUser();
	Task LoginUser();
	Task LogoutUser();
	Task SetUser(StringValues code);
	Task<bool> IsUserSaved();
}