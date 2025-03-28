using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ILoginService
{
	Task AutoLoginUser();
	Task LoginUser();
	Task LogoutUser();
	Task SetUser(StringValues code);
	Task<bool> IsUserSaved();
}