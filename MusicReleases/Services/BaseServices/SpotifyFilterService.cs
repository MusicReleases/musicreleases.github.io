using Fluxor;
using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Store.FilterStore;
using JakubKastner.SpotifyApi.Objects;
using System.Diagnostics;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyFilterService(IState<SpotifyFilterState> filterState) : ISpotifyFilterService
{
	private readonly IState<SpotifyFilterState> _filterState = filterState;


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

		var sw = Stopwatch.StartNew();

		var (filteredReleasesByTypeDate, filteredReleasesByTypeArtist) = FilterReleases();
		FilterDate(filteredReleasesByTypeArtist);
		FilterArtists(filteredReleasesByTypeDate);

		OnFilterOrDataChanged?.Invoke();

		sw.Stop();
		Console.WriteLine($"Filtrace 2 - {_allReleases.Count} trvá {sw.ElapsedMilliseconds} ms");
	}

	private (ISet<SpotifyRelease> byTypeDate, ISet<SpotifyRelease> byTypeArtist) FilterReleases()
	{
		if (_allReleases is null)
		{
			throw new NullReferenceException(nameof(_allReleases));
		}

		var filter = _filterState.Value.Filter;

		// releases by type
		var releasesByType = _allReleases.Where(r => r.ReleaseType == filter.ReleaseType);

		// releases by artist
		var releasesByTypeArtist
			= filter.Artist.IsNullOrEmpty()
			? releasesByType
			: releasesByType.Where(r => r.Artists.Any(a => a.Id == filter.Artist));


		// releases by date
		IEnumerable<SpotifyRelease>? releasesByTypeArtistDate = null;
		IEnumerable<SpotifyRelease>? releasesByTypeDate = null;

		if (filter.Month.HasValue)
		{
			releasesByTypeArtistDate = releasesByTypeArtist.Where(r => r.ReleaseDate.Month == filter.Month.Value.Month && r.ReleaseDate.Year == filter.Month.Value.Year);
			releasesByTypeDate = releasesByType.Where(r => r.ReleaseDate.Month == filter.Month.Value.Month && r.ReleaseDate.Year == filter.Month.Value.Year);
		}
		else if (filter.Year.HasValue)
		{
			releasesByTypeArtistDate = releasesByTypeArtist.Where(r => r.ReleaseDate.Year == filter.Year);
			releasesByTypeDate = releasesByType.Where(r => r.ReleaseDate.Year == filter.Year);
		}
		else
		{
			releasesByTypeArtistDate = releasesByTypeArtist;
			releasesByTypeDate = releasesByType;
		}

		FilteredReleases = [.. releasesByTypeArtistDate];

		return (releasesByTypeDate.ToHashSet(), releasesByTypeArtist.ToHashSet());
	}

	private void FilterArtists(ISet<SpotifyRelease> releasesByTypeDate)
	{
		var filter = _filterState.Value.Filter;

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
}
