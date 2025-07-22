using Fluxor;
using JakubKastner.MusicReleases.Objects;

namespace JakubKastner.MusicReleases.Store.FilterStore;

[FeatureState]
public class SpotifyFilterState
{
	public SpotifyFilter Filter { get; }

	public SpotifyFilterState()
	{
		Filter = new();
	}

	public SpotifyFilterState(SpotifyFilter filter)
	{
		Filter = filter;
	}
}