using Fluxor;

namespace JakubKastner.MusicReleases.Store.Releases;

public class ReducerReleasesLoaded : Reducer<StateReleases, ActionReleasesLoaded>
{
    public override StateReleases Reduce(StateReleases state, ActionReleasesLoaded action)
    {
        return new(true);
    }
}
