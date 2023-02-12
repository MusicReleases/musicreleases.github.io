using Fluxor;

namespace JakubKastner.MusicReleases.Store.Releases;

public class LoaderFeature : Feature<LoaderState>
{
    public override string GetName()
    {
        return "Loader";
    }

    protected override LoaderState GetInitialState()
    {
        return new()
        {
            Loading = false,
        };
    }
}
