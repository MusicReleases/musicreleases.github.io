using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyFilterService(IDbSpotifyFilterService filterDbService, ISpotifyUserService spotifyUserService) : ISpotifyFilterService
{
	private readonly IDbSpotifyFilterService _filterDbService = filterDbService;
	private readonly ISpotifyUserService _spotifyUserService = spotifyUserService;

	public SpotifyFilter? Filter { get; private set; } = null;

	private ISet<SpotifyRelease>? _allReleases = null;
	private ISet<SpotifyArtist>? _allArtists = null;

	public SortedSet<SpotifyRelease> FilteredReleases { get; private set; } = [];
	public SortedSet<SpotifyArtist>? FilteredArtists { get; private set; } = null;
	public Dictionary<int, SortedSet<int>>? FilteredYearMonth { get; private set; } = null;

	public event Action? OnFilterOrDataChanged;

	public void SetArtists(ISet<SpotifyArtist> artists)
	{
		_allArtists = artists;
		// clear releases when artists are set
		_allReleases = null;
		ApplyFilter();
	}
	public void SetReleases(ISet<SpotifyRelease> releases)
	{
		_allReleases = releases;
		ApplyFilter();
	}

	private void ApplyFilter()
	{
		if (_allReleases is null && _allArtists is null)
		{
			return;
		}

		if (_allReleases is null)
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
		if (Filter is null)
		{
			throw new NullReferenceException(nameof(Filter));
		}

		if (_allReleases is null)
		{
			throw new NullReferenceException(nameof(_allReleases));
		}

		// releases by type
		var releasesByType = _allReleases.Where(r => r.ReleaseType == Filter.ReleaseType);

		// releases by advanced filter
		var releasesByTypeAdvanced = FilterReleasesAdvanced(releasesByType);

		// releases by artist
		var releasesByTypeAdvancedArtist
			= Filter.Artist.IsNullOrEmpty()
			? releasesByTypeAdvanced
			: releasesByTypeAdvanced.Where(r => r.Artists.Any(a => a.Id == Filter.Artist));

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

		return (releasesByTypeAdvancedDate.ToHashSet(), releasesByTypeAdvancedArtist.ToHashSet());
	}

	private IEnumerable<SpotifyRelease> FilterReleasesAdvanced(IEnumerable<SpotifyRelease> releasesByType)
	{
		if (Filter is null)
		{
			throw new NullReferenceException(nameof(Filter));
		}

		var releasesByTypeAdvanced = releasesByType;
		if (Filter.ReleaseType == SpotifyEnums.ReleaseType.Tracks || Filter.ReleaseType == SpotifyEnums.ReleaseType.Appears)
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
		if (_allArtists is null)
		{
			FilteredArtists = null;
			return;
		}

		var artistIdsInFilteredReleases = releasesByTypeDate.SelectMany(r => r.Artists.Select(a => a.Id)).ToHashSet();
		var filteredArtists = _allArtists.Where(a => artistIdsInFilteredReleases.Contains(a.Id));

		FilteredArtists = [.. filteredArtists];
	}

	private void FilterDate(ISet<SpotifyRelease> releases)
	{
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
	}

	public void SetFilter(SpotifyFilter filter)
	{
		Filter = filter;
	}

	public async Task SetFilterAndSaveDb(SpotifyFilter filter)
	{
		var userId = _spotifyUserService.GetUserIdRequired();
		await _filterDbService.Save(filter, userId);
		SetFilter(filter);
	}
}
