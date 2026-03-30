using JakubKastner.MusicReleases.Spotify.Releases.User;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases;

internal sealed class SpotifyReleaseFilterUrlService(ISpotifyReleaseFilterService filterService) : ISpotifyReleaseFilterUrlService
{
	private readonly ISpotifyReleaseFilterService _filterService = filterService;


	private const string _urlSeparator = "/";
	private const string _urlNull = "_";

	public string CreateUrl(SpotifyReleaseFilter filter)
	{
		var urlPathList = new List<string>
		{
			filter.ReleaseGroup.ToLowerString(),
			filter.Year.HasValue ? filter.Year.Value.ToString() : _urlNull,
			filter.Month.HasValue ? filter.Month.Value.Date.Month.ToString() : _urlNull,
			filter.Artist.IsNotNullOrEmpty() ? filter.Artist : _urlNull
		};

		var urlPath = string.Join(_urlSeparator, urlPathList);
		var urlParams = CreateUrlParams(filter.ReleaseAdvancedFilter, filter.SearchText);

		var url = $"{urlPath}{urlParams}";
		return url;
	}

	private string CreateUrlParams(ReleaseAdvancedFilter advancedFilter, string? searchText)
	{
		var urlParams = new List<string>();

		if (advancedFilter != ReleaseAdvancedFilter.All)
		{
			var flags = Enum.GetValues<ReleaseAdvancedFilter>().Where(f => f != ReleaseAdvancedFilter.All && advancedFilter.HasFlag(f));

			urlParams.Add("filter=" + string.Join(",", flags));
		}

		if (searchText.IsNotNullOrEmpty())
		{
			urlParams.Add("search=" + searchText);
		}

		if (urlParams.Count == 0)
		{
			return string.Empty;
		}

		return $"?{string.Join("&", urlParams)}";
	}

	public SpotifyReleaseFilter ParseFilterFromUrlParams(SpotifyReleaseUrlParameters urlParameters)
	{
		if (!Enum.TryParse(urlParameters.Type, true, out ReleaseGroup releaseType))
		{
			releaseType = ReleaseGroup.Albums;
		}
		int? year = int.TryParse(urlParameters.Year, out var yearParsed) ? yearParsed : null;
		int? monthValue = int.TryParse(urlParameters.Month, out var monthParsed) ? monthParsed : null;
		DateTime? month = year.HasValue ? (monthValue.HasValue ? new(year.Value, monthValue.Value, 1) : null) : null;
		var artist = urlParameters.ArtistId == _urlNull ? null : urlParameters.ArtistId;
		var searchText = _filterService.EnsureSearchText(urlParameters.Search);

		var advancedFilter = ParseAdvancedFilterFromUrlParams(urlParameters.Filter);
		return new(releaseType, advancedFilter, artist, year, month, searchText);
	}

	private ReleaseAdvancedFilter ParseAdvancedFilterFromUrlParams(string? advancedFilterParams)
	{

		if (advancedFilterParams.IsNullOrEmpty())
		{
			return ReleaseAdvancedFilter.All;
		}

		var filter = (ReleaseAdvancedFilter)0;

		var parts = advancedFilterParams.Split(',', StringSplitOptions.RemoveEmptyEntries);

		foreach (var p in parts)
		{
			if (Enum.TryParse<ReleaseAdvancedFilter>(p, true, out var parsed))
			{
				filter |= parsed;
			}
		}

		filter = _filterService.EnsureAdvancedFilter(filter);

		return filter;
	}
}

