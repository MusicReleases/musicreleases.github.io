using Fluxor;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.FilterStore;

[FeatureState]
public class SpotifyFilterState
{
	public ISet<SpotifyRelease>? AllReleases { get; }
	public ISet<SpotifyRelease>? FilteredReleases { get; }
	public ISet<SpotifyArtist>? AllArtists { get; }
	public ISet<SpotifyArtist>? FilteredArtists { get; }
	public Dictionary<int, SortedSet<int>>? FilteredYearMonth { get; }
	public SpotifyFilter Filter { get; }

	public SpotifyFilterState()
	{
		AllReleases = null;
		FilteredReleases = null;
		AllArtists = null;
		FilteredArtists = null;
		Filter = new();
	}

	public SpotifyFilterState(ISet<SpotifyRelease> allReleases, ISet<SpotifyRelease> filteredReleases, ISet<SpotifyArtist>? allArtists, ISet<SpotifyArtist>? filteredArtists, Dictionary<int, SortedSet<int>>? filteredYearMonth, SpotifyFilter filter)
	{
		AllReleases = allReleases;
		FilteredReleases = filteredReleases;
		AllArtists = allArtists;
		FilteredArtists = filteredArtists;
		FilteredYearMonth = filteredYearMonth;
		Filter = filter;
	}

	public SpotifyFilterState(ISet<SpotifyArtist> allArtists, SpotifyFilter filter)
	{
		AllReleases = null;
		FilteredReleases = null;
		AllArtists = allArtists;
		FilteredArtists = allArtists;
		Filter = filter;
	}

	public SpotifyFilterState(SpotifyFilter filter)
	{
		AllReleases = null;
		FilteredReleases = null;
		AllArtists = null;
		FilteredArtists = null;
		Filter = filter;
	}
}