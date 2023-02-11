using Fluxor;

namespace JakubKastner.MusicReleases.Store.Releases;

public class FeatureReleases : Feature<StateReleases>
{
    public override string GetName()
    {
        return "Releases";
    }

    protected override StateReleases GetInitialState()
    {
        return new(false);
    }
}
