using Microsoft.Extensions.Primitives;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.BaseControllers;

public interface ILoginController
{
	Task AutoLoginUser(ServiceType serviceType);
	Task LoginUser(ServiceType serviceType);
	Task SetUser(ServiceType serviceType, StringValues code);
	Task<bool> IsUserSaved(ServiceType serviceType);
}