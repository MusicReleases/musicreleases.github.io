using Fluxor;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.FilterStore;

[FeatureState]
public class SpotifyFilterState
{
	public ISet<SpotifyRelease> AllReleases { get; }
	public ISet<SpotifyRelease> FilteredReleases { get; }
	public SpotifyFilter Filter { get; }

	public SpotifyFilterState()
	{
		AllReleases = new SortedSet<SpotifyRelease>();
		FilteredReleases = new SortedSet<SpotifyRelease>();
		Filter = new();
	}
	public SpotifyFilterState(ISet<SpotifyRelease> allReleases, ISet<SpotifyRelease> filteredReleases, SpotifyFilter filter)
	{
		AllReleases = allReleases;
		FilteredReleases = filteredReleases;
		Filter = filter;
	}
}