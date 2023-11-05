using JakubKastner.MusicReleases.Base;

namespace JakubKastner.MusicReleases.Controllers.BaseControllers
{
	public interface IServiceTypeController
	{
		Enums.ServiceType? Get();
		Enums.ServiceType GetRequired();
		void Remove();
		void Set(Enums.ServiceType serviceType);
	}
}