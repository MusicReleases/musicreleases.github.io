using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyReleaseFilterUrlService() : ISpotifyReleaseFilterUrlService
{

	private const string _urlSeparator = "/";
	private const string _urlNull = "_";


	public string CreateUrl(MainReleasesType releaseType, string? year, string? month, string? artist, ReleaseAdvancedFilter advancedFilter, string? searchText)
	{
		var urlPathList = new List<string>
		{
			releaseType.ToLowerString(),
			year.IsNullOrEmpty() ? _urlNull : year,
			month.IsNullOrEmpty() ? _urlNull : month,
			artist.IsNullOrEmpty() ? _urlNull : artist
		};

		var urlPath = string.Join(_urlSeparator, urlPathList);
		var urlParams = CreateUrlParams(advancedFilter, searchText);

		var url = $"{urlPath}{urlParams}";
		return url;
	}

	public string CreateUrl(SpotifyFilter filter)
	{
		var urlPathList = new List<string>
		{
			filter.ReleaseType.ToLowerString(),
			filter.Year.HasValue ? filter.Year.Value.ToString() : _urlNull,
			filter.Month.HasValue ? filter.Month.Value.Date.Month.ToString() : _urlNull,
			filter.Artist.IsNotNullOrEmpty() ? filter.Artist : _urlNull
		};

		var urlPath = string.Join(_urlSeparator, urlPathList);
		var urlParams = CreateUrlParams(filter.ReleaseAdvancedFilter, filter.SearchText);

		var url = $"{urlPath}{urlParams}";
		return url;
	}

	private static string CreateUrlParams(ReleaseAdvancedFilter advancedFilter, string? searchText)
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

	public SpotifyFilter ParseFilterFromUrlParams(string? releaseType, string? year, string? month, string? artist, string? advancedFilterParams)
	{
		if (!Enum.TryParse(releaseType, true, out MainReleasesType releaseTypeFilter))
		{
			releaseTypeFilter = MainReleasesType.Albums;
		}
		int? yearFilter = int.TryParse(year, out var yearValue) ? yearValue : null;
		int? monthInt = int.TryParse(month, out var monthValue) ? monthValue : null;
		DateTime? monthFilter = yearFilter.HasValue ? (monthInt.HasValue ? new(yearFilter.Value, monthInt.Value, 1) : null) : null;
		var artistFilter = artist == _urlNull ? null : artist;


		var advancedFilter = ParseAdvancedFilterFromUrlParams(advancedFilterParams);
		return new(releaseTypeFilter, advancedFilter, artistFilter, yearFilter, monthFilter);
	}

	private static ReleaseAdvancedFilter ParseAdvancedFilterFromUrlParams(string? advancedFilterParams)
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
		return filter;
	}
}

