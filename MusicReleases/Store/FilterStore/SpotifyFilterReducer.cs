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
		return FilterReleases(action.Releases, action.Artists, state.Filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetReleaseTypeFilterAction(SpotifyFilterState state, SetReleaseTypeFilterAction action)
	{
		var filter = new SpotifyFilter(state.Filter)
		{
			ReleaseType = action.ReleaseType,
		};
		return FilterReleases(state.AllReleases, state.AllArtists, filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetArtistFilterAction(SpotifyFilterState state, SetArtistFilterAction action)
	{
		var filter = new SpotifyFilter(state.Filter)
		{
			Artist = action.ArtistId,
		};
		return FilterReleases(state.AllReleases, state.AllArtists, filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetYearFilterAction(SpotifyFilterState state, SetYearFilterAction action)
	{
		var filter = new SpotifyFilter(state.Filter)
		{
			Year = action.Year,
		};
		return FilterReleases(state.AllReleases, state.AllArtists, filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetMonthFilterAction(SpotifyFilterState state, SetMonthFilterAction action)
	{
		var filter = new SpotifyFilter(state.Filter)
		{
			Month = action.Month,
		};
		return FilterReleases(state.AllReleases, state.AllArtists, filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceResetFiltersAction(SpotifyFilterState state, ResetFiltersAction action)
	{
		var filter = new SpotifyFilter();
		return FilterReleases(state.AllReleases, state.AllArtists, filter);
		//return new(state.AllReleases, state.AllReleases, filter);
	}

	[ReducerMethod]
	public static SpotifyFilterState ReduceSetFiltersAction(SpotifyFilterState state, SetFiltersAction action)
	{
		return FilterReleases(state.AllReleases, state.AllArtists, action.Filter);
	}

	private static SpotifyFilterState FilterReleases(ISet<SpotifyRelease> allReleases, ISet<SpotifyArtist> allArtists, SpotifyFilter filter)
	{
		var filteredReleases = allReleases.Where(r => r.ReleaseType == filter.ReleaseType);
		IEnumerable<SpotifyRelease>? filteredReleases1 = null;
		if (filter.Year.HasValue)
		{
			filteredReleases1 = filteredReleases.Where(r => r.ReleaseDate.Year == filter.Year.Value);
		}

		if (filter.Month.HasValue)
		{
			filteredReleases1 = filteredReleases.Where(r => r.ReleaseDate.Month == filter.Month.Value.Month && r.ReleaseDate.Year == filter.Month.Value.Year);
		}

		filteredReleases1 ??= filteredReleases;

		var artistIdsInFilteredReleases = filteredReleases1.SelectMany(r => r.Artists.Select(a => a.Id));
		var filteredArtists = allArtists.Where(a => artistIdsInFilteredReleases.Contains(a.Id));


		if (filter.Artist.IsNotNullOrEmpty())
		{
			filteredReleases = filteredReleases.Where(r => r.Artists.Any(a => a.Id == filter.Artist));
		}

		// year + month for date menu
		var filteredDate = new SortedSet<DateTime>(filteredReleases.Select(x => x.ReleaseDate));
		var filteredYearMonth = filteredDate?.Select(x => x.Year).Distinct().OrderByDescending(x => x).ToDictionary
				(
					year => year,
					year => new SortedSet<int>
					(
						filteredDate.Where(d => d.Year == year).Select(d => d.Month),
						Comparer<int>.Create((x, y) => y.CompareTo(x))
					)
				);

		if (filter.Year.HasValue)
		{
			filteredReleases = filteredReleases.Where(r => r.ReleaseDate.Year == filter.Year.Value);
		}

		if (filter.Month.HasValue)
		{
			filteredReleases = filteredReleases.Where(r => r.ReleaseDate.Month == filter.Month.Value.Month && r.ReleaseDate.Year == filter.Month.Value.Year);
		}

		var filteredReleasesSet = new SortedSet<SpotifyRelease>(filteredReleases);
		var filteredArtistsSet = new SortedSet<SpotifyArtist>(filteredArtists);

		return new(allReleases, filteredReleasesSet, allArtists, filteredArtistsSet, filteredYearMonth, filter);
	}
}