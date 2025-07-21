using Fluxor;
using JakubKastner.MusicReleases.Objects;
using static JakubKastner.MusicReleases.Store.FilterStore.SpotifyFilterAction;

namespace JakubKastner.MusicReleases.Store.FilterStore;

public class SpotifyFilterReducer
{
	[ReducerMethod]
	public static SpotifyFilterState ReduceSetReleaseTypeFilterAction(SpotifyFilterState state, SetReleaseTypeFilterAction action)
	{
		var filter = new SpotifyFilter(state.Filter)
		{
			ReleaseType = action.ReleaseType,
		};
		return FilterReleases(filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetArtistFilterAction(SpotifyFilterState state, SetArtistFilterAction action)
	{
		var filter = new SpotifyFilter(state.Filter)
		{
			Artist = action.ArtistId,
		};
		return FilterReleases(filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetYearFilterAction(SpotifyFilterState state, SetYearFilterAction action)
	{
		var filter = new SpotifyFilter(state.Filter)
		{
			Year = action.Year,
		};
		return FilterReleases(filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetMonthFilterAction(SpotifyFilterState state, SetMonthFilterAction action)
	{
		var filter = new SpotifyFilter(state.Filter)
		{
			Month = action.Month,
		};
		return FilterReleases(filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceResetFiltersAction(SpotifyFilterState state, ResetFiltersAction action)
	{
		var filter = new SpotifyFilter();
		return FilterReleases(filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetFiltersAction(SpotifyFilterState state, SetFiltersAction action)
	{
		return FilterReleases(action.Filter);
	}
	private static SpotifyFilterState FilterReleases(SpotifyFilter filter)
	{
		return new(filter);
	}
}