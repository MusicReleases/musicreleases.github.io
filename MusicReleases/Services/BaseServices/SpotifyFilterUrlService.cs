using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Objects;
using static JakubKastner.MusicReleases.Base.Enums;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyFilterUrlService(ISpotifyFilterService spotifyFilterService) : ISpotifyFilterUrlService
{
	private readonly ISpotifyFilterService _spotifyFilterService = spotifyFilterService;
	private const string _urlNull = "_";

	private SpotifyFilter Filter => _spotifyFilterService.Filter;

	private string GetFilterUrl(string? releaseTypeUrl, string? yearUrl, string? monthUrl, string? artistUrl, ReleasesFilters? advancedFilterType = null, bool advancedFilterActive = false)
	{
		const string urlSeparator = "/";
		const string urlRelease = "releases";

		monthUrl = yearUrl.IsNotNullOrEmpty() ? monthUrl : null;

		var releaseType = releaseTypeUrl ?? Filter.ReleaseType.ToString().ToLower();
		var year = yearUrl ?? (Filter.Year.HasValue ? Filter.Year.Value.ToString() : (Filter.Month.HasValue ? Filter.Month.Value.Year.ToString() : _urlNull));
		var month = monthUrl ?? (Filter.Month.HasValue ? Filter.Month.Value.Month.ToString() : _urlNull);
		var artist = artistUrl ?? Filter.Artist ?? _urlNull;

		var urlParams = new List<string>
		{
			urlRelease,
			releaseType,
			year,
			month,
			artist
		};

		var url = string.Join(urlSeparator, urlParams);

		// advanced filter
		var advancedFilterUrl = GetAdvancedFilterUrl(advancedFilterType, advancedFilterActive);
		if (advancedFilterUrl.IsNotNullOrEmpty())
		{
			url += advancedFilterUrl;
		}

		return url;
	}

	private string GetAdvancedFilterUrl(ReleasesFilters? advancedFilterType, bool advancedFilterActive)
	{
		const string urlNull = "";
		const string urlSeparator = "&";

		if (advancedFilterType.HasValue && advancedFilterType.Value == ReleasesFilters.Clear)
		{
			return urlNull;
		}

		// get url from current advanced filter
		var advancedFilterActiveUrl = advancedFilterType.HasValue && advancedFilterActive ? advancedFilterType.Value.ToString().ToLower() : urlNull;

		var tracks = advancedFilterType == ReleasesFilters.Tracks ? advancedFilterActiveUrl : (Filter.Advanced.Tracks == true ? ReleasesFilters.Tracks.ToString().ToLower() : urlNull);
		var eps = advancedFilterType == ReleasesFilters.EPs ? advancedFilterActiveUrl : (Filter.Advanced.EPs == true ? ReleasesFilters.EPs.ToString().ToLower() : urlNull);
		var remixes = advancedFilterType == ReleasesFilters.Remixes ? advancedFilterActiveUrl : (Filter.Advanced.Remixes == true ? ReleasesFilters.Remixes.ToString().ToLower() : urlNull);
		var followedArtists = advancedFilterType == ReleasesFilters.FollowedArtists ? advancedFilterActiveUrl : (Filter.Advanced.FollowedArtists == true ? ReleasesFilters.FollowedArtists.ToString().ToLower() : urlNull);
		var variousArtists = advancedFilterType == ReleasesFilters.VariousArtists ? advancedFilterActiveUrl : (Filter.Advanced.VariousArtists == true ? ReleasesFilters.VariousArtists.ToString().ToLower() : urlNull);
		var inLibrary = advancedFilterType == ReleasesFilters.InLibrary ? advancedFilterActiveUrl : (Filter.Advanced.InLibrary == true ? ReleasesFilters.InLibrary.ToString().ToLower() : urlNull);
		var onlyNew = advancedFilterType == ReleasesFilters.OnlyNew ? advancedFilterActiveUrl : (Filter.Advanced.OnlyNew == true ? ReleasesFilters.OnlyNew.ToString().ToLower() : urlNull);

		var urlParams = new List<string>
		{
			tracks,
			eps,
			remixes,
			followedArtists,
			variousArtists,
			inLibrary,
			onlyNew,
		};

		var url = string.Join(urlSeparator, urlParams);
		return $"?{url}";
	}

	public string GetFilterUrl()
	{
		return GetFilterUrl(null, null, null, null);
	}
	public string GetFilterUrl(ReleaseType releaseType)
	{
		var releaseTypeUrl = releaseType.ToString().ToLower();
		return GetFilterUrl(releaseTypeUrl, null, null, null);
	}
	public string GetFilterUrl(int? year)
	{
		var yearUrl = year.HasValue ? year.Value.ToString() : _urlNull;
		var monthUrl = _urlNull;
		return GetFilterUrl(null, yearUrl, monthUrl, null);
	}
	public string GetFilterUrl(int? year, int? month)
	{
		var yearUrl = year.HasValue ? year.Value.ToString() : _urlNull;
		var monthUrl = month.HasValue ? month.Value.ToString() : _urlNull;
		return GetFilterUrl(null, yearUrl, monthUrl, null);
	}
	public string GetFilterUrl(string? artist)
	{
		var artistUrl = artist.IsNotNullOrEmpty() ? artist : _urlNull;
		return GetFilterUrl(null, null, null, artistUrl);
	}
	public string GetFilterUrl(ReleasesFilters advancedFilterType, bool advancedFilterActive)
	{
		return GetFilterUrl(null, null, null, null, advancedFilterType, advancedFilterActive);
	}

	public SpotifyFilter ParseFilterUrl(string? releaseType, string? year, string? month, string? artist, SpotifyFilterAdvanced advancedFilter)
	{
		if (!Enum.TryParse(releaseType, true, out ReleaseType type))
		{
			type = ReleaseType.Albums;
		}
		int? yearFilter = int.TryParse(year, out var yearValue) ? yearValue : null;
		int? monthInt = int.TryParse(month, out var monthValue) ? monthValue : null;
		DateTime? monthFilter = yearFilter.HasValue ? (monthInt.HasValue ? new(yearFilter.Value, monthInt.Value, 1) : null) : null;
		var artistFilter = artist == _urlNull ? null : artist;

		return new(type, yearFilter, monthFilter, artistFilter, advancedFilter);
	}

	public string ClearFilter(MenuButtonsType type)
	{
		// TODO custom enum
		return type switch
		{
			MenuButtonsType.Date => GetFilterUrl(year: null, month: null),
			MenuButtonsType.Artists => GetFilterUrl(artist: null),
			MenuButtonsType.Releases => GetFilterUrl(advancedFilterType: ReleasesFilters.Clear, advancedFilterActive: true),
			_ => throw new NotSupportedException(nameof(MenuButtonsType)),
		};
	}
}
