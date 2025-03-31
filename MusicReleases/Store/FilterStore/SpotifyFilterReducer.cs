using Fluxor;
using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.MusicReleases.Store.FilterStore.SpotifyFilterAction;

namespace JakubKastner.MusicReleases.Store.FilterStore;

public class SpotifyFilterReducer
{
	[ReducerMethod]
	public static SpotifyFilterState ReduceLoadReleasesAction(SpotifyFilterState state, LoadReleasesAction action)
	{
		var filteredReleases = FilterReleases(action.Releases, state.Filter);

		return new(action.Releases, filteredReleases, state.Filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetReleaseTypeFilterAction(SpotifyFilterState state, SetReleaseTypeFilterAction action)
	{
		var filter = new SpotifyFilter(state.Filter)
		{
			ReleaseType = action.ReleaseType,
		};
		var filteredReleases = FilterReleases(state.AllReleases, filter);

		return new(state.AllReleases, filteredReleases, filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetArtistFilterAction(SpotifyFilterState state, SetArtistFilterAction action)
	{
		var filter = new SpotifyFilter(state.Filter)
		{
			Artist = action.ArtistId,
		};
		var filteredReleases = FilterReleases(state.AllReleases, filter);

		return new(state.AllReleases, filteredReleases, filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetYearFilterAction(SpotifyFilterState state, SetYearFilterAction action)
	{
		var filter = new SpotifyFilter(state.Filter)
		{
			Year = action.Year,
		};
		var filteredReleases = FilterReleases(state.AllReleases, filter);

		return new(state.AllReleases, filteredReleases, filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetMonthFilterAction(SpotifyFilterState state, SetMonthFilterAction action)
	{
		var filter = new SpotifyFilter(state.Filter)
		{
			Month = action.Month,
		};
		var filteredReleases = FilterReleases(state.AllReleases, filter);

		return new(state.AllReleases, filteredReleases, filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceResetFiltersAction(SpotifyFilterState state, ResetFiltersAction action)
	{
		var filter = new SpotifyFilter();
		return new(state.AllReleases, state.AllReleases, filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetFiltersAction(SpotifyFilterState state, SetFiltersAction action)
	{
		var filteredReleases = FilterReleases(state.AllReleases, action.Filter);
		return new(state.AllReleases, filteredReleases, action.Filter);
	}

	private static ISet<SpotifyRelease> FilterReleases(ISet<SpotifyRelease> allReleases, SpotifyFilter filter)
	{
		var filtered = allReleases.Where(r => r.ReleaseType == filter.ReleaseType);

		if (filter.Artist.IsNotNullOrEmpty())
		{
			filtered = filtered.Where(r => r.Artists.Any(a => a.Id == filter.Artist));
		}

		if (filter.Year.HasValue)
		{
			filtered = filtered.Where(r => r.ReleaseDate.Year == filter.Year.Value);
		}

		if (filter.Month.HasValue)
		{
			filtered = filtered.Where(r => r.ReleaseDate.Month == filter.Month.Value.Month && r.ReleaseDate.Year == filter.Month.Value.Year);
		}

		return new SortedSet<SpotifyRelease>(filtered);
	}
}