using Microsoft.Extensions.Primitives;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.Base;

public interface ILoginController
{
	Task LoginUser(ServiceType type);
	Task SetUser(ServiceType type, StringValues code);
	Task<bool> IsUserSaved(ServiceType type);
}