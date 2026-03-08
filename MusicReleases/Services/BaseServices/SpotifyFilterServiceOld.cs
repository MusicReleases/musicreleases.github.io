using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyFilterServiceOld(IDbSpotifyReleaseFilterService filterDbService, ISpotifyApiUserService spotifyUserService) : ISpotifyFilterServiceOld
{
	private readonly IDbSpotifyReleaseFilterService _filterDbService = filterDbService;
	private readonly ISpotifyApiUserService _spotifyUserService = spotifyUserService;

	public SpotifyReleaseFilter? Filter { get; private set; } = null;

	private Dictionary<MainReleasesType, ISet<SpotifyRelease>>? _allReleasesByType = null;
	private ISet<SpotifyArtist>? _allArtists = null;

	public SortedSet<SpotifyRelease> FilteredReleases { get; private set; } = [];
	public SortedSet<SpotifyArtist>? FilteredArtists { get; private set; } = null;
	public Dictionary<int, SortedSet<int>>? FilteredYearMonth { get; private set; } = null;

	public event Action? OnFilterOrDataChanged;

	public void SetArtists(ISet<SpotifyArtist> artists)
	{
		_allArtists = artists;
		// clear releases when artists are set
		_allReleasesByType = null;
		ApplyFilter();
	}

	public void SetReleases(Dictionary<MainReleasesType, IReadOnlyList<SpotifyRelease>> releasesByType)
	{
		_allReleasesByType = [];
		foreach (var kvp in releasesByType)
		{
			// create new copy
			_allReleasesByType[kvp.Key] = kvp.Value.ToHashSet();
		}
		ApplyFilter();
	}

	private void ApplyFilter()
	{
		if (_allReleasesByType is null && _allArtists is null)
		{
			return;
		}

		if (_allReleasesByType is null)
		{
			FilteredArtists = [.. _allArtists!];
			OnFilterOrDataChanged?.Invoke();
			return;
		}

		if (Filter is null)
		{
			throw new NullReferenceException(nameof(Filter));
		}

		var (filteredReleasesByTypeDate, filteredReleasesByTypeArtist) = FilterReleases();
		FilterDate(filteredReleasesByTypeArtist);
		FilterArtists(filteredReleasesByTypeDate);

		OnFilterOrDataChanged?.Invoke();
	}

	private (ISet<SpotifyRelease> byTypeDate, ISet<SpotifyRelease> byTypeArtist) FilterReleases()
	{
		Console.WriteLine("filter: releases - start");
		if (Filter is null)
		{
			throw new NullReferenceException(nameof(Filter));
		}

		if (_allReleasesByType is null)
		{
			throw new NullReferenceException(nameof(_allReleasesByType));
		}

		// releases by type
		var releasesByType = _allReleasesByType.TryGetValue(Filter.ReleaseType, out var set) ? set : new HashSet<SpotifyRelease>();

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
		if (Filter.ReleaseType == MainReleasesType.Tracks || Filter.ReleaseType == MainReleasesType.Appears)
		{
			// tracks and eps filter only for tracks and appears
			if (!Filter.Advanced.Tracks)
			{
				// display only eps
				releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.TotalTracks > 1);
			}
			if (!Filter.Advanced.EPs)
			{
				// display only tracks
				releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.TotalTracks == 1);
			}
		}
		if (!Filter.Advanced.NotRemixes)
		{
			// display only remixes
			releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.Name.Contains("remix", StringComparison.CurrentCultureIgnoreCase) || r.Name.Contains("rmx", StringComparison.CurrentCultureIgnoreCase));
		}
		if (!Filter.Advanced.Remixes)
		{
			// display only not remixes
			releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => !r.Name.Contains("remix", StringComparison.CurrentCultureIgnoreCase) && !r.Name.Contains("rmx", StringComparison.CurrentCultureIgnoreCase));
		}
		if (!Filter.Advanced.SavedReleases)
		{
			// TODO display only releases from followed artists
		}
		if (!Filter.Advanced.FollowedArtists)
		{
			// TODO display only saved releases
		}
		if (!Filter.Advanced.NotVariousArtists)
		{
			// display only releases with various artists
			releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.Artists.Any(a => a.Name == "Various Artists"));
		}
		if (!Filter.Advanced.VariousArtists)
		{
			// display only releases without various artists
			releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => !r.Artists.Any(a => a.Name == "Various Artists"));
		}
		if (!Filter.Advanced.OldReleases)
		{
			// display only new releases
			releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => r.New);
		}
		if (!Filter.Advanced.NewReleases)
		{
			// display only old releases
			releasesByTypeAdvanced = releasesByTypeAdvanced.Where(r => !r.New);
		}
		return releasesByTypeAdvanced;
	}

	private void FilterArtists(ISet<SpotifyRelease> releasesByTypeDate)
	{
		if (Filter is null)
		{
			throw new NullReferenceException(nameof(Filter));
		}

		Console.WriteLine("filter: artists - start");

		if (_allArtists is null)
		{
			FilteredArtists = null;
			return;
		}

		var artistIdsInFilteredReleases
			= Filter.ReleaseType == MainReleasesType.Appears
			? releasesByTypeDate.SelectMany(r => r.FeaturedArtists.Select(a => a.Id)).ToHashSet()
			: releasesByTypeDate.SelectMany(r => r.Artists.Select(a => a.Id)).ToHashSet();

		var filteredArtists = _allArtists.Where(a => artistIdsInFilteredReleases.Contains(a.Id));

		FilteredArtists = [.. filteredArtists];
		Console.WriteLine("filter: artists - end");
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

		FilteredYearMonth = filteredYearMonth;
		Console.WriteLine("filter: date - end");
	}

	public void SetFilter(SpotifyReleaseFilter filter)
	{
		Filter = filter;

		//OnFilterOrDataChanged?.Invoke();
	}

	public async Task SetFilterAndSaveDb(SpotifyReleaseFilter filter)
	{
		var userId = _spotifyUserService.GetUserIdRequired();
		await _filterDbService.Save(filter, userId);
		SetFilter(filter);
	}

	public void ClearFilter()
	{
		Filter = new();
	}

	public bool IsFilterActive(FilterType filterType)
	{
		if (Filter is null)
		{
			return false;
		}

		return filterType switch
		{
			FilterType.Any => IsFilterActive(FilterType.Artist) || IsFilterActive(FilterType.Date) || IsFilterActive(FilterType.Advanced),
			FilterType.Artist => Filter.Artist.IsNotNullOrEmpty(),
			FilterType.Date => Filter.Year is not null,
			FilterType.Advanced => Filter.Advanced.IsActive,
			_ => throw new NotImplementedException(),
		};
	}
}
