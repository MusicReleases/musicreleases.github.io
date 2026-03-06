using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyReleaseFilterService : IDisposable, ISpotifyReleaseFilterService
{
	private readonly ISpotifyReleaseState _releaseState;

	private readonly ISpotifyArtistState _artistState;

	public SpotifyReleaseFilterService(ISpotifyReleaseState releaseState, ISpotifyArtistState artistState)
	{
		_releaseState = releaseState;
		_artistState = artistState;

		_releaseState.OnChange += OnReleaseStateChanged;
		_artistState.OnChange += OnArtistStateChanged;
	}

	public void Dispose()
	{
		_releaseState.OnChange -= OnReleaseStateChanged;
		_artistState.OnChange -= OnArtistStateChanged;
		GC.SuppressFinalize(this);
	}


	public event Action? OnFilterOrDataChanged;

	public SpotifyFilter Filter { get; private set; } = new();


	public SortedSet<SpotifyRelease>? FilteredReleases { get; private set; } = null;

	public SortedSet<SpotifyArtist>? FilteredArtists { get; private set; } = null;

	public Dictionary<int, SortedSet<int>>? FilteredDate { get; private set; } = null;


	private ConcurrentDictionary<MainReleasesType, IReadOnlyList<SpotifyRelease>> AllReleases => _releaseState.ReleasesByType;

	private IReadOnlyList<SpotifyArtist> AllArtists => _artistState.SortedFollowedArtists;


	private const ReleaseAdvancedFilter _defaultAdvancedFilter = ReleaseAdvancedFilter.All;

	private static readonly ReleaseAdvancedFilter[][] AdvancedFilterGroups =
	[
		[ReleaseAdvancedFilter.Albums,              ReleaseAdvancedFilter.Tracks,           ReleaseAdvancedFilter.EPs],
		[ReleaseAdvancedFilter.NotRemixes,          ReleaseAdvancedFilter.Remixes],
		[ReleaseAdvancedFilter.FollowedArtists,     ReleaseAdvancedFilter.SavedReleases],
		[ReleaseAdvancedFilter.NotVariousArtists,   ReleaseAdvancedFilter.VariousArtists],
		[ReleaseAdvancedFilter.OldReleases,         ReleaseAdvancedFilter.NewReleases],
	];


	private static readonly Dictionary<ReleaseAdvancedFilter, Func<SpotifyRelease, bool>> FilterPredicates =
	new()
	{
		[ReleaseAdvancedFilter.NotRemixes] = t => !t.Name.Contains("remix", StringComparison.CurrentCultureIgnoreCase) || t.Name.Contains("rmx", StringComparison.CurrentCultureIgnoreCase),
		[ReleaseAdvancedFilter.Remixes] = t => t.Name.Contains("remix", StringComparison.CurrentCultureIgnoreCase) || t.Name.Contains("rmx", StringComparison.CurrentCultureIgnoreCase),

		// TODO display only saved releases o only releases with various artists
		[ReleaseAdvancedFilter.FollowedArtists] = t => t.Id is not null,
		[ReleaseAdvancedFilter.SavedReleases] = t => t.Id is not null,

		[ReleaseAdvancedFilter.NotVariousArtists] = t => !t.Artists.Any(a => a.Name == "Various Artists"),
		[ReleaseAdvancedFilter.VariousArtists] = t => t.Artists.Any(a => a.Name == "Various Artists"),

		[ReleaseAdvancedFilter.OldReleases] = t => !t.New,
		[ReleaseAdvancedFilter.NewReleases] = t => t.New,
	};

	private void OnReleaseStateChanged()
	{
		ApplyFilterAndSearch();
	}

	private void OnArtistStateChanged()
	{
		ApplyFilterAndSearch();
	}

	public void ApplyFilterAndSearch()
	{
		var filterApplied = ApplyFilter();
		var searchApplied = ApplySearch();

		if (filterApplied || searchApplied)
		{
			OnFilterOrDataChanged?.Invoke();
		}
	}


	private bool ApplyFilter()
	{
		if (AllReleases.IsEmpty && AllArtists.Count == 0)
		{
			return false;
		}

		if (AllReleases is null)
		{
			FilteredArtists = [.. AllArtists];
			//OnFilterOrDataChanged?.Invoke();
			return true;
		}

		var (filteredReleasesByTypeDate, filteredReleasesByTypeArtist) = FilterReleases();
		FilterDate(filteredReleasesByTypeArtist);
		FilterArtists(filteredReleasesByTypeDate);

		//OnFilterOrDataChanged?.Invoke();
		return true;
	}



	private (ISet<SpotifyRelease> byTypeDate, ISet<SpotifyRelease> byTypeArtist) FilterReleases()
	{
		Console.WriteLine("filter: releases - start");
		if (Filter is null)
		{
			throw new NullReferenceException(nameof(Filter));
		}

		// releases by type
		var releasesByType = AllReleases.TryGetValue(Filter.ReleaseType, out var set) ? set : [];

		// releases by advanced filter
		var releasesByTypeAdvanced = FilterReleasesAdvanced(releasesByType);

		// releases by artist
		var releasesByTypeAdvancedArtist = releasesByTypeAdvanced;
		if (Filter.Artist.IsNotNullOrEmpty())
		{
			if (Filter.ReleaseType == MainReleasesType.Appears)
			{
				releasesByTypeAdvancedArtist = releasesByTypeAdvanced.Where(r => r.FeaturedArtists.Any(a => a.Id == Filter.Artist));
			}
			else
			{
				releasesByTypeAdvancedArtist = releasesByTypeAdvanced.Where(r => r.Artists.Any(a => a.Id == Filter.Artist));
			}
		}

		// releases by date
		IEnumerable<SpotifyRelease>? releasesByTypeAdvancedArtistDate = null;
		IEnumerable<SpotifyRelease>? releasesByTypeAdvancedDate = null;

		if (Filter.Month.HasValue)
		{
			releasesByTypeAdvancedArtistDate = releasesByTypeAdvancedArtist.Where(r => r.ReleaseDate.Month == Filter.Month.Value.Month && r.ReleaseDate.Year == Filter.Month.Value.Year);
			releasesByTypeAdvancedDate = releasesByTypeAdvanced.Where(r => r.ReleaseDate.Month == Filter.Month.Value.Month && r.ReleaseDate.Year == Filter.Month.Value.Year);
		}
		else if (Filter.Year.HasValue)
		{
			releasesByTypeAdvancedArtistDate = releasesByTypeAdvancedArtist.Where(r => r.ReleaseDate.Year == Filter.Year);
			releasesByTypeAdvancedDate = releasesByTypeAdvanced.Where(r => r.ReleaseDate.Year == Filter.Year);
		}
		else
		{
			releasesByTypeAdvancedArtistDate = releasesByTypeAdvancedArtist;
			releasesByTypeAdvancedDate = releasesByTypeAdvanced;
		}

		FilteredReleases = [.. releasesByTypeAdvancedArtistDate];
		Console.WriteLine("filter: releases - end");

		return (releasesByTypeAdvancedDate.ToHashSet(), releasesByTypeAdvancedArtist.ToHashSet());
	}


	private IEnumerable<SpotifyRelease> FilterReleasesAdvanced(IEnumerable<SpotifyRelease> releasesByType)
	{
		if (Filter is null)
		{
			throw new NullReferenceException(nameof(Filter));
		}

		var releasesByTypeAdvanced = releasesByType;

		// tracks and eps filter only for tracks and appears
		if (Filter.ReleaseType == MainReleasesType.Tracks)
		{
			if (Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Tracks) && Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Tracks))
			{
				// display all
			}
			else if (Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Tracks))
			{
				// display only tracks
				releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.TotalTracks == 1);

			}
			else
			{
				// display only eps
				releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.TotalTracks > 1);
			}
		}

		// albums filter only for tracks and appears
		if (Filter.ReleaseType == MainReleasesType.Appears)
		{
			if (Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Tracks)
				&& Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.EPs)
				&& Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Albums))
			{
				// display all
			}
			else if (Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Tracks)
				&& !Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.EPs)
				&& !Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Albums))
			{
				// display only tracks
				releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.TotalTracks == 1);
			}
			else if (!Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Tracks)
				&& Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.EPs)
				&& !Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Albums))
			{
				// display only eps
				releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.TotalTracks > 1 && r.ReleaseType == ReleaseType.Track);
			}
			else if (!Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Tracks)
				&& !Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.EPs)
				&& Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Albums))
			{
				// display only albums
				releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.TotalTracks > 1 && r.ReleaseType != ReleaseType.Track);
			}
			else if (Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Tracks)
				&& Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.EPs)
				&& !Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Albums))
			{
				// display tracks and eps 
				releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.ReleaseType == ReleaseType.Track);
			}
			else if (Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Tracks)
				&& !Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.EPs)
				&& Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Albums))
			{
				// display tracks and albums 
				releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.TotalTracks == 1 || r.ReleaseType == ReleaseType.Album);
			}
			else if (!Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Tracks)
				&& Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.EPs)
				&& Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Albums))
			{
				// display eps and albums
				releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.TotalTracks > 1);

			}
		}


		/*if (Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Remixes) && !Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.NotRemixes))
		{
			// display only remixes
			releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.Name.Contains("remix", StringComparison.CurrentCultureIgnoreCase) || r.Name.Contains("rmx", StringComparison.CurrentCultureIgnoreCase));
		}
		else if (!Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Remixes) && Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.NotRemixes))
		{
			// display only not remixes
			releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => !r.Name.Contains("remix", StringComparison.CurrentCultureIgnoreCase) && !r.Name.Contains("rmx", StringComparison.CurrentCultureIgnoreCase));
		}


		if (Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.SavedReleases) && !Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.FollowedArtists))
		{
			// TODO display only saved releases
		}
		else if (!Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.SavedReleases) && Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.FollowedArtists))
		{
			// TODO display only releases from followed artists
		}

		if (Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.VariousArtists) && !Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.NotVariousArtists))
		{
			// display only releases with various artists
			releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.Artists.Any(a => a.Name == "Various Artists"));
		}
		else if (!Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.VariousArtists) && Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.NotVariousArtists))
		{
			// display only releases without various artists
			releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => !r.Artists.Any(a => a.Name == "Various Artists"));
		}


		if (Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.NewReleases) && !Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.OldReleases))
		{
			// display only new releases
			releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.New);
		}
		else if (!Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.NewReleases) && Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.OldReleases))
		{
			// display only old releases
			releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => !r.New);
		}*/

		var releasesAdvanced = ApplyAdvancedFilter(releasesByTypeAdvanced);

		return releasesAdvanced;
	}

	private IEnumerable<SpotifyRelease> ApplyAdvancedFilter(IEnumerable<SpotifyRelease> source)
	{
		if (Filter.ReleaseAdvancedFilter.HasFlag(_defaultAdvancedFilter))
		{
			return source;
		}

		var query = source;

		foreach (var group in AdvancedFilterGroups)
		{
			query = ApplyAdvancedFilterGroup(query, group);
		}

		return query;
	}


	private IEnumerable<SpotifyRelease> ApplyAdvancedFilterGroup(IEnumerable<SpotifyRelease> source, ReleaseAdvancedFilter[] group)
	{
		var active = group.Where(f => Filter.ReleaseAdvancedFilter.HasFlag(f)).ToArray();

		if (active.Length == 1)
		{
			var only = active[0];
			return source.Where(FilterPredicates[only]);
		}

		return source;
	}





	private void FilterDate(ISet<SpotifyRelease> releases)
	{
		Console.WriteLine("filter: date - start");
		var filteredYearMonth = releases
			.Select(r => r.ReleaseDate)
			.Distinct()
			.GroupBy(date => date.Year)
			.OrderByDescending(g => g.Key)
			.ToDictionary(
				g => g.Key, // year
				g => new SortedSet<int>(
					g.Select(date => date.Month).Distinct(),
					Comparer<int>.Create((x, y) => y.CompareTo(x))
				)
			);

		FilteredDate = filteredYearMonth;
		Console.WriteLine("filter: date - end");
	}

	private void FilterArtists(ISet<SpotifyRelease> releasesByTypeDate)
	{
		Console.WriteLine("filter: artists - start");

		if (AllArtists.Count == 0)
		{
			FilteredArtists = null;
			return;
		}

		var artistIdsInFilteredReleases
			= Filter.ReleaseType == MainReleasesType.Appears
			? releasesByTypeDate.SelectMany(r => r.FeaturedArtists.Select(a => a.Id)).ToHashSet()
			: releasesByTypeDate.SelectMany(r => r.Artists.Select(a => a.Id)).ToHashSet();

		var filteredArtists = AllArtists.Where(a => artistIdsInFilteredReleases.Contains(a.Id));

		FilteredArtists = [.. filteredArtists];
		Console.WriteLine("filter: artists - end");
	}


	public bool IsAdvancedFilterActive(ReleaseAdvancedFilter advancedFilter)
	{
		return Filter.ReleaseAdvancedFilter.HasFlag(advancedFilter);
	}


	public void ToggleAdvancedFilter(ReleaseAdvancedFilter advancedFilter)
	{
		var newFilter = Filter.ReleaseAdvancedFilter ^ advancedFilter;

		SetAdvancedFilterInternal(newFilter);
	}
	public void SeAdvancedFilter(ReleaseAdvancedFilter advancedFilter)
	{
		var newFilter = Filter.ReleaseAdvancedFilter | advancedFilter;

		SetAdvancedFilterInternal(newFilter);
	}

	public void UnsetAdvancedFilter(ReleaseAdvancedFilter advancedFilter)
	{
		var newFilter = Filter.ReleaseAdvancedFilter & ~advancedFilter;

		SetAdvancedFilterInternal(newFilter);
	}


	private static ReleaseAdvancedFilter EnsureAdvnacedFilterGroups(ReleaseAdvancedFilter newFilter)
	{
		foreach (var group in AdvancedFilterGroups)
		{
			var anyActive = group.Any(f => newFilter.HasFlag(f));
			if (!anyActive)
			{
				foreach (var f in group)
				{
					newFilter |= f;
				}
			}
		}
		return newFilter;
	}

	private bool SetAdvancedFilterInternal(ReleaseAdvancedFilter newFilter)
	{
		var newFilterEnsured = EnsureAdvnacedFilterGroups(newFilter);

		if (newFilterEnsured == Filter.ReleaseAdvancedFilter)
		{
			return false;
		}

		Filter.ReleaseAdvancedFilter = newFilterEnsured;

		ApplyFilterAndSearch();

		//OnFilterOrDataChanged?.Invoke();
		return true;
	}

	private bool SetFilterInternal(SpotifyFilter newFilter, bool onChange = true)
	{
		var advancedFilter = EnsureAdvnacedFilterGroups(newFilter.ReleaseAdvancedFilter);
		newFilter.ReleaseAdvancedFilter = advancedFilter;

		if (newFilter == Filter)
		{
			return false;
		}

		Filter = newFilter;


		if (onChange)
		{
			ApplyFilterAndSearch();
			//OnFilterOrDataChanged?.Invoke();
		}
		return true;
	}


	public void ClearAllFilters()
	{
		SetFilterInternal(new());
	}

	public void ClearAdvancedFilter()
	{
		SetAdvancedFilterInternal(_defaultAdvancedFilter);
	}

	public bool IsFilterActive(FilterType filterType)
	{
		return filterType switch
		{
			FilterType.Any => IsFilterActive(FilterType.Artist) || IsFilterActive(FilterType.Date) || IsFilterActive(FilterType.Advanced),
			FilterType.Artist => Filter.Artist.IsNotNullOrEmpty(),
			FilterType.Date => Filter.Year is not null,
			FilterType.Advanced => !IsAdvancedFilterActive(_defaultAdvancedFilter),
			_ => throw new NotImplementedException(),
		};
	}


	public void SetFilterAndSearch(SpotifyFilter newFilter, string? newSearchText, bool onChange = false)
	{
		SetFilterAndSearchInternal(newFilter, newSearchText, onChange);
	}

	private void SetFilterAndSearchInternal(SpotifyFilter newFilter, string? newSearchText, bool onChange = false)
	{
		var filterChanged = SetFilterInternal(newFilter, false);
		var searchChanged = SetSearchInternal(newSearchText, false);

		if (onChange || filterChanged || searchChanged)
		{
			OnFilterOrDataChanged?.Invoke();
		}
	}

	private static string? EnsureSearchText(string? searchText)
	{
		return searchText.IsNullOrEmpty() ? null : searchText.Trim();
	}

	public void SetSearch(string? newSearchText)
	{
		SetSearchInternal(newSearchText);
	}

	private bool SetSearchInternal(string? newSearchText, bool onChange = true)
	{
		var searchText = EnsureSearchText(newSearchText);

		if (string.Equals(searchText, Filter.SearchText, StringComparison.OrdinalIgnoreCase))
		{
			return false;
		}

		Filter.SearchText = newSearchText;

		if (onChange)
		{
			OnFilterOrDataChanged?.Invoke();
		}
		return true;
	}

	private bool ApplySearch()
	{
		if (FilteredReleases is null || Filter.SearchText.IsNullOrEmpty())
		{
			return false;
		}

		var query = FilteredReleases.Where(t => t.Name.Contains(Filter.SearchText, StringComparison.InvariantCultureIgnoreCase));

		FilteredReleases = [.. query];

		return true;
	}


	public void ClearFilter(FilterType type)
	{
		switch (type)
		{
			case FilterType.Any:
				ClearAllFilters();
				break;
			case FilterType.Artist:
				Filter.Artist = null;
				break;
			case FilterType.Date:
				Filter.Year = null;
				Filter.Month = null;
				break;
			case FilterType.Advanced:
				ClearAdvancedFilter();
				break;
			default:
				throw new NotImplementedException();
		}
	}

	public void FilterArtist(string? artistId)
	{
		if (string.Equals(artistId, Filter.Artist, StringComparison.OrdinalIgnoreCase))
		{
			artistId = null;
		}

		Filter.Artist = artistId;
		ApplyFilterAndSearch();
	}
	public void FilterYear(int? year)
	{
		FilterMonth(year, null);
	}
	public void FilterMonth(int? year, int? month)
	{
		if (Filter.Year == year && Filter.Month?.Month == month)
		{
			year = null;
			month = null;
		}
		Filter.Year = year;
		Filter.Month = year.HasValue && month.HasValue ? new(year.Value, month.Value, 1) : null;

		ApplyFilterAndSearch();
	}
	public void FilterReleaseType(MainReleasesType releaseType)
	{
		if (releaseType == Filter.ReleaseType)
		{
			return;
		}

		Filter.ReleaseType = releaseType;
		ApplyFilterAndSearch();
	}
}