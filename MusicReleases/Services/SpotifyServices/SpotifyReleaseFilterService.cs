using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

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

	public event Action? OnFilterChanged;

	public event Action? OnDataFiltered;

	public event Action? NotifySynchronizer;


	public SpotifyReleaseFilter Filter { get; private set; } = new();


	public SortedSet<SpotifyRelease>? FilteredReleases { get; private set; } = null;

	public SortedSet<SpotifyArtist>? FilteredArtists { get; private set; } = null;

	public Dictionary<int, SortedSet<int>>? FilteredDate { get; private set; } = null;


	private ConcurrentDictionary<ReleaseGroup, IReadOnlyList<SpotifyRelease>> AllReleases => _releaseState.ReleasesByType;

	private IReadOnlyList<SpotifyArtist> AllArtists => _artistState.SortedFollowedArtists;


	private const ReleaseAdvancedFilter _defaultAdvancedFilter = ReleaseAdvancedFilter.All;

	private static readonly ReleaseAdvancedFilter[][] AdvancedFilterGroups =
	[
		[ReleaseAdvancedFilter.Albums,              ReleaseAdvancedFilter.Tracks,           ReleaseAdvancedFilter.EPs,           ReleaseAdvancedFilter.Compilations],
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
			OnDataFiltered?.Invoke();
			OnFilterChanged?.Invoke();
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
			return true;
		}

		var (filteredReleasesByTypeDate, filteredReleasesByTypeArtist) = FilterReleases();
		FilterDate(filteredReleasesByTypeArtist);
		FilterArtists(filteredReleasesByTypeDate);

		return true;
	}

	private (ISet<SpotifyRelease> byTypeDate, ISet<SpotifyRelease> byTypeArtist) FilterReleases()
	{
		Console.WriteLine("filter: releases - start");

		// releases by type
		var releasesByType = AllReleases.TryGetValue(Filter.ReleaseType, out var set) ? set : [];

		// releases by advanced filter
		var releasesByTypeAdvanced = ApplyAdvancedFilter(releasesByType);

		// releases by artist
		var releasesByTypeAdvancedArtist = releasesByTypeAdvanced;
		if (Filter.Artist.IsNotNullOrEmpty())
		{
			if (Filter.ReleaseType == ReleaseGroup.Appears)
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

	private IEnumerable<SpotifyRelease> ApplyAdvancedFilter(IEnumerable<SpotifyRelease> releasesByType)
	{
		var query = ApplyAdvancedFilterAlbumEpTrack(releasesByType);
		query = ApplyAdvancedFilterOther(query);
		return query;
	}


	private IEnumerable<SpotifyRelease> ApplyAdvancedFilterAlbumEpTrack(IEnumerable<SpotifyRelease> releasesByType)
	{
		if (Filter.ReleaseAdvancedFilter.HasFlag(_defaultAdvancedFilter))
		{
			return releasesByType;
		}
		var query = releasesByType;

		var showAlbums = Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Albums);
		var showTracks = Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Tracks);
		var showEPs = Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.EPs);
		var showCompilations = Filter.ReleaseAdvancedFilter.HasFlag(ReleaseAdvancedFilter.Compilations);

		if (Filter.ReleaseType == ReleaseGroup.Tracks)
		{
			if (showTracks ^ showEPs)
			{
				query = query.Where(r => showTracks ? r.TotalTracks == 1 : r.TotalTracks > 1);
			}
		}
		else if (Filter.ReleaseType == ReleaseGroup.Appears)
		{
			var anySelected = showAlbums || showTracks || showEPs || showCompilations;
			var allSelected = showAlbums && showTracks && showEPs && showCompilations;

			if (anySelected && !allSelected)
			{
				query = query.Where(r =>
					(showTracks && r.TotalTracks == 1 && r.ReleaseType == ReleaseType.Track) ||
					(showEPs && r.TotalTracks > 1 && r.ReleaseType == ReleaseType.Track) ||
					(showAlbums && r.ReleaseType == ReleaseType.Album) ||
					(showCompilations && r.ReleaseType == ReleaseType.Compilation)
				);
			}
		}

		return query;
	}

	private IEnumerable<SpotifyRelease> ApplyAdvancedFilterOther(IEnumerable<SpotifyRelease> source)
	{
		if (Filter.ReleaseAdvancedFilter.HasFlag(_defaultAdvancedFilter))
		{
			return source;
		}

		var query = source;

		// skip first group because it's already applied in ApplyAdvancedFilterAlbumEpTrack
		foreach (var group in AdvancedFilterGroups.Skip(1))
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
			= Filter.ReleaseType == ReleaseGroup.Appears
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


	public ReleaseAdvancedFilter EnsureAdvancedFilter(ReleaseAdvancedFilter newFilter)
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
		var newFilterEnsured = EnsureAdvancedFilter(newFilter);

		if (newFilterEnsured == Filter.ReleaseAdvancedFilter)
		{
			return false;
		}

		Filter.ReleaseAdvancedFilter = newFilterEnsured;

		OnFilterChanged?.Invoke();
		NotifySynchronizer?.Invoke();

		return true;
	}

	private bool SetFilterInternal(SpotifyReleaseFilter newFilter, bool onChange = true)
	{
		var advancedFilter = EnsureAdvancedFilter(newFilter.ReleaseAdvancedFilter);
		newFilter.ReleaseAdvancedFilter = advancedFilter;

		if (newFilter == Filter)
		{
			return false;
		}

		Filter = newFilter;


		if (onChange)
		{
			OnFilterChanged?.Invoke();
			NotifySynchronizer?.Invoke();
		}
		return true;
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


	public void SetFromUrl(SpotifyReleaseFilter newFilter)
	{
		var filterChanged = SetFilterInternal(newFilter, false);
		var searchChanged = SetSearchInternal(newFilter.SearchText, false);

		if (filterChanged || searchChanged)
		{
			ApplyFilterAndSearch();
		}
	}

	public void SetSearch(string? searchText)
	{
		SetSearchInternal(searchText);
	}

	public string? EnsureSearchText(string? searchText)
	{
		return searchText.EnsureText();
	}

	public void EnsureFilter(SpotifyReleaseFilter filter)
	{
		filter.ReleaseAdvancedFilter = EnsureAdvancedFilter(filter.ReleaseAdvancedFilter);
		filter.SearchText = EnsureSearchText(filter.SearchText);
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
			OnFilterChanged?.Invoke();
			NotifySynchronizer?.Invoke();
		}
		return true;
	}

	private bool ApplySearch()
	{
		if (FilteredReleases is null || Filter.SearchText.IsNullOrEmpty())
		{
			return false;
		}

		var query = FilteredReleases.ApplySearch(Filter.SearchText, t => t.Name);

		FilteredReleases = [.. query];

		return true;
	}

	public void ClearFilter(FilterType type)
	{
		switch (type)
		{
			case FilterType.Any:
				SetFilterInternal(new(Filter.ReleaseType));
				break;
			case FilterType.Artist:
				Filter.Artist = null;
				OnFilterChanged?.Invoke();
				NotifySynchronizer?.Invoke();
				break;
			case FilterType.Date:
				Filter.Year = null;
				Filter.Month = null;
				OnFilterChanged?.Invoke();
				NotifySynchronizer?.Invoke();
				break;
			case FilterType.Advanced:
				SetAdvancedFilterInternal(_defaultAdvancedFilter);
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

		OnFilterChanged?.Invoke();
		NotifySynchronizer?.Invoke();
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

		OnFilterChanged?.Invoke();
		NotifySynchronizer?.Invoke();
	}

	public void FilterReleaseType(ReleaseGroup releaseType)
	{
		if (releaseType == Filter.ReleaseType)
		{
			return;
		}

		Filter.ReleaseType = releaseType;

		OnFilterChanged?.Invoke();
		NotifySynchronizer?.Invoke();
	}
}