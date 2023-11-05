using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.BaseControllers;
public class ServiceTypeController : IServiceTypeController
{
    private ServiceType? _serviceType;

    public void Set(ServiceType serviceType)
    {
        _serviceType = serviceType;
    }

    public void Remove()
    {
        _serviceType = null;
    }

    public ServiceType? Get()
    {
        return _serviceType;
    }

    public ServiceType GetRequired()
    {
        if (!_serviceType.HasValue)
        {
            throw new NullReferenceException(nameof(_serviceType));
        }

        return _serviceType.Value;
    }
}
