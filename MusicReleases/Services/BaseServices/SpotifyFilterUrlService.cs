using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Services;
using static JakubKastner.MusicReleases.Base.Enums;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyFilterUrlService(ISpotifyFilterService spotifyFilterService, IDbSpotifyFilterService dbSpotifyFilterService, ISpotifyUserService spotifyUserService) : ISpotifyFilterUrlService
{
	private readonly ISpotifyFilterService _spotifyFilterService = spotifyFilterService;
	private readonly IDbSpotifyFilterService _dbSpotifyFilterService = dbSpotifyFilterService;
	private readonly ISpotifyUserService _spotifyUserService = spotifyUserService;

	private const string _urlNull = "_";

	private SpotifyFilter? Filter => _spotifyFilterService.Filter;

	private async Task<string> GetFilterUrl(string? releaseTypeUrl, string? yearUrl, string? monthUrl, string? artistUrl, ReleasesFilters? advancedFilterType = null, bool advancedFilterActive = false)
	{
		if (Filter is null)
		{
			var userId = _spotifyUserService.GetUserIdRequired();

			// get filter from database or create new
			var filterDb = await _dbSpotifyFilterService.Get(userId) ?? new();
			_spotifyFilterService.SetFilter(filterDb);
		}

		const string urlSeparator = "/";
		const string urlRelease = "releases";

		monthUrl = yearUrl.IsNotNullOrEmpty() ? monthUrl : null;

		var releaseType = releaseTypeUrl ?? Filter!.ReleaseType.ToString().ToLower();
		var year = yearUrl ?? (Filter!.Year.HasValue ? Filter.Year.Value.ToString() : (Filter.Month.HasValue ? Filter.Month.Value.Year.ToString() : _urlNull));
		var month = monthUrl ?? (Filter!.Month.HasValue ? Filter.Month.Value.Month.ToString() : _urlNull);
		var artist = artistUrl ?? Filter!.Artist ?? _urlNull;

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
		if (Filter is null)
		{
			throw new NullReferenceException(nameof(Filter));
		}

		const string urlNull = "";
		const string urlSeparator = "&";

		if (advancedFilterType.HasValue && advancedFilterType.Value == ReleasesFilters.Clear)
		{
			return GetAdvancedFilterUrlDefault();
		}

		// get url from current advanced filter
		var advancedFilterActiveUrl = advancedFilterType.HasValue && advancedFilterActive ? advancedFilterType.Value.ToString().ToLower() : urlNull;

		var tracks = advancedFilterType == ReleasesFilters.Tracks ? advancedFilterActiveUrl : (Filter.Advanced.Tracks == true ? ReleasesFilters.Tracks.ToString().ToLower() : urlNull);
		var eps = advancedFilterType == ReleasesFilters.EPs ? advancedFilterActiveUrl : (Filter.Advanced.EPs == true ? ReleasesFilters.EPs.ToString().ToLower() : urlNull);
		var notRemixes = advancedFilterType == ReleasesFilters.NotRemixes ? advancedFilterActiveUrl : (Filter.Advanced.NotRemixes == true ? ReleasesFilters.NotRemixes.ToString().ToLower() : urlNull);
		var remixes = advancedFilterType == ReleasesFilters.Remixes ? advancedFilterActiveUrl : (Filter.Advanced.Remixes == true ? ReleasesFilters.Remixes.ToString().ToLower() : urlNull);
		var followedArtists = advancedFilterType == ReleasesFilters.FollowedArtists ? advancedFilterActiveUrl : (Filter.Advanced.FollowedArtists == true ? ReleasesFilters.FollowedArtists.ToString().ToLower() : urlNull);
		var savedReleases = advancedFilterType == ReleasesFilters.SavedReleases ? advancedFilterActiveUrl : (Filter.Advanced.SavedReleases == true ? ReleasesFilters.SavedReleases.ToString().ToLower() : urlNull);
		var notVariousArtists = advancedFilterType == ReleasesFilters.NotVariousArtists ? advancedFilterActiveUrl : (Filter.Advanced.NotVariousArtists == true ? ReleasesFilters.NotVariousArtists.ToString().ToLower() : urlNull);
		var variousArtists = advancedFilterType == ReleasesFilters.VariousArtists ? advancedFilterActiveUrl : (Filter.Advanced.VariousArtists == true ? ReleasesFilters.VariousArtists.ToString().ToLower() : urlNull);
		var newReleases = advancedFilterType == ReleasesFilters.NewReleases ? advancedFilterActiveUrl : (Filter.Advanced.NewReleases == true ? ReleasesFilters.NewReleases.ToString().ToLower() : urlNull);
		var oldReleases = advancedFilterType == ReleasesFilters.OldReleases ? advancedFilterActiveUrl : (Filter.Advanced.OldReleases == true ? ReleasesFilters.OldReleases.ToString().ToLower() : urlNull);

		var urlParams = new List<string>
		{
			tracks,
			eps,
			notRemixes,
			remixes,
			followedArtists,
			savedReleases,
			notVariousArtists,
			variousArtists,
			newReleases,
			oldReleases,
		}.Where(x => x.IsNotNullOrEmpty());

		if (urlParams.Any())
		{
			var url = string.Join(urlSeparator, urlParams);
			return $"?{url}";
		}

		// empty advanced filter
		return GetAdvancedFilterUrlDefault();
	}

	private static string GetAdvancedFilterUrlDefault()
	{
		const string urlSeparator = "&";
		var urlParams = new List<string>
		{
			ReleasesFilters.Tracks.ToString().ToLower(),
			ReleasesFilters.EPs.ToString().ToLower(),
			ReleasesFilters.NotRemixes.ToString().ToLower(),
			ReleasesFilters.Remixes.ToString().ToLower(),
			ReleasesFilters.FollowedArtists.ToString().ToLower(),
			//ReleasesFilters.SavedReleases.ToString().ToLower(),
			ReleasesFilters.NotVariousArtists.ToString().ToLower(),
			ReleasesFilters.VariousArtists.ToString().ToLower(),
			ReleasesFilters.NewReleases.ToString().ToLower(),
			ReleasesFilters.OldReleases.ToString().ToLower(),
		};
		var urlDefault = string.Join(urlSeparator, urlParams);
		return $"?{urlDefault}";
	}

	public async Task<string> GetFilterUrl()
	{
		return await GetFilterUrl(null, null, null, null);
	}
	public async Task<string> GetFilterUrl(ReleaseType releaseType)
	{
		var releaseTypeUrl = releaseType.ToString().ToLower();
		return await GetFilterUrl(releaseTypeUrl, null, null, null);
	}
	public async Task<string> GetFilterUrl(int? year)
	{
		var yearUrl = year.HasValue ? year.Value.ToString() : _urlNull;
		var monthUrl = _urlNull;
		return await GetFilterUrl(null, yearUrl, monthUrl, null);
	}
	public async Task<string> GetFilterUrl(int? year, int? month)
	{
		var yearUrl = year.HasValue ? year.Value.ToString() : _urlNull;
		var monthUrl = month.HasValue ? month.Value.ToString() : _urlNull;
		return await GetFilterUrl(null, yearUrl, monthUrl, null);
	}
	public async Task<string> GetFilterUrl(string? artist)
	{
		var artistUrl = artist.IsNotNullOrEmpty() ? artist : _urlNull;
		return await GetFilterUrl(null, null, null, artistUrl);
	}
	public async Task<string> GetFilterUrl(ReleasesFilters advancedFilterType, bool advancedFilterActive)
	{
		return await GetFilterUrl(null, null, null, null, advancedFilterType, advancedFilterActive);
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

	public async Task<string> ClearFilter(MenuButtonsType type)
	{
		// TODO custom enum
		return type switch
		{
			MenuButtonsType.Date => await GetFilterUrl(year: null, month: null),
			MenuButtonsType.Artists => await GetFilterUrl(artist: null),
			MenuButtonsType.Releases => await GetFilterUrl(advancedFilterType: ReleasesFilters.Clear, advancedFilterActive: true),
			_ => throw new NotSupportedException(nameof(MenuButtonsType)),
		};
	}
}
