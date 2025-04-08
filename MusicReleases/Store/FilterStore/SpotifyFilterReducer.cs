using Fluxor;
using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.MusicReleases.Store.FilterStore.SpotifyFilterAction;

namespace JakubKastner.MusicReleases.Store.FilterStore;

public class SpotifyFilterReducer
{
	[ReducerMethod]
	public static SpotifyFilterState ReduceLoadArtistsAction(SpotifyFilterState state, LoadArtistsAction action)
	{
		return FilterReleases(null, action.Artists, state.Filter);
	}
	[ReducerMethod]
	public static SpotifyFilterState ReduceLoadReleasesAction(SpotifyFilterState state, LoadReleasesAction action)
	{
		return FilterReleases(action.Releases, state.AllArtists, state.Filter);
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

	private static SpotifyFilterState FilterReleases(ISet<SpotifyRelease>? allReleases, ISet<SpotifyArtist>? allArtists, SpotifyFilter filter)
	{
		if (allReleases is null && allArtists is null)
		{
			return new SpotifyFilterState(filter);
		}

		if (allReleases is null)
		{
			return new(allArtists!, filter);
		}

		var filteredReleasesByType = allReleases.Where(r => r.ReleaseType == filter.ReleaseType);
		IEnumerable<SpotifyRelease>? filteredReleasesByDateAndType = null;
		if (filter.Year.HasValue)
		{
			filteredReleasesByDateAndType = filteredReleasesByType.Where(r => r.ReleaseDate.Year == filter.Year.Value);
		}

		if (filter.Month.HasValue)
		{
			filteredReleasesByDateAndType = filteredReleasesByType.Where(r => r.ReleaseDate.Month == filter.Month.Value.Month && r.ReleaseDate.Year == filter.Month.Value.Year);
		}

		filteredReleasesByDateAndType ??= filteredReleasesByType;

		var artistIdsInFilteredReleases = filteredReleasesByDateAndType.SelectMany(r => r.Artists.Select(a => a.Id));

		IEnumerable<SpotifyArtist>? filteredArtists = null;
		if (allArtists is not null)
		{
			filteredArtists = allArtists.Where(a => artistIdsInFilteredReleases.Contains(a.Id));
		}


		if (filter.Artist.IsNotNullOrEmpty())
		{
			filteredReleasesByType = filteredReleasesByType.Where(r => r.Artists.Any(a => a.Id == filter.Artist));
		}

		// year + month for date menu
		var filteredYearMonth = GetFilteredYearAndMonth(filteredReleasesByType);

		if (filter.Year.HasValue)
		{
			filteredReleasesByType = filteredReleasesByType.Where(r => r.ReleaseDate.Year == filter.Year.Value);
		}

		if (filter.Month.HasValue)
		{
			filteredReleasesByType = filteredReleasesByType.Where(r => r.ReleaseDate.Month == filter.Month.Value.Month && r.ReleaseDate.Year == filter.Month.Value.Year);
		}

		var filteredReleasesSet = new SortedSet<SpotifyRelease>(filteredReleasesByType);
		var filteredArtistsSet = filteredArtists is null ? null : new SortedSet<SpotifyArtist>(filteredArtists);

		return new(allReleases, filteredReleasesSet, allArtists, filteredArtistsSet, filteredYearMonth, filter);
	}

	private static Dictionary<int, SortedSet<int>>? GetFilteredYearAndMonth(IEnumerable<SpotifyRelease> releases)
	{
		// year + month for date menu
		var filteredDate = new SortedSet<DateTime>(releases.Select(x => x.ReleaseDate));
		var filteredYearMonth = filteredDate?.Select(x => x.Year).Distinct().OrderByDescending(x => x).ToDictionary
				(
					year => year,
					year => new SortedSet<int>
					(
						filteredDate.Where(d => d.Year == year).Select(d => d.Month),
						Comparer<int>.Create((x, y) => y.CompareTo(x))
					)
				);

		return filteredYearMonth;
	}
}