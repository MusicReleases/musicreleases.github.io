using Fluxor;
using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Base;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.MusicReleases.Store.FilterStore;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class SpotifyFilterUrlService(IState<SpotifyFilterState> spotifyFilterState) : ISpotifyFilterUrlService
{
	private readonly IState<SpotifyFilterState> _spotifyFilterState = spotifyFilterState;
	private const string _urlNull = "_";

	private SpotifyFilter Filter => _spotifyFilterState.Value.Filter;

	private string GetFilterUrl(string? releaseTypeUrl, string? yearUrl, string? monthUrl, string? artistUrl)
	{
		var urlParams = new List<string>();

		const string urlSeparator = "/";
		const string urlRelease = "releases";

		monthUrl = yearUrl.IsNotNullOrEmpty() ? monthUrl : null;

		var releaseType = releaseTypeUrl ?? Filter.ReleaseType.ToString().ToLower();
		var year = yearUrl ?? (Filter.Year.HasValue ? Filter.Year.Value.ToString() : (Filter.Month.HasValue ? Filter.Month.Value.Year.ToString() : _urlNull));
		var month = monthUrl ?? (Filter.Month.HasValue ? Filter.Month.Value.Month.ToString() : _urlNull);
		var artist = artistUrl ?? Filter.Artist ?? _urlNull;

		urlParams.Add(urlRelease);
		urlParams.Add(releaseType);
		urlParams.Add(year);
		urlParams.Add(month);
		urlParams.Add(artist);

		var url = string.Join(urlSeparator, urlParams);
		return url;
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

	public SpotifyFilter ParseFilterUrl(string? releaseType, string? year, string? month, string? artist)
	{
		if (!Enum.TryParse(releaseType, true, out ReleaseType type))
		{
			type = ReleaseType.Albums;
		}
		int? yearFilter = int.TryParse(year, out var yearValue) ? yearValue : null;
		int? monthInt = int.TryParse(month, out var monthValue) ? monthValue : null;
		DateTime? monthFilter = yearFilter.HasValue ? (monthInt.HasValue ? new(yearFilter.Value, monthInt.Value, 1) : null) : null;
		var artistFilter = artist == _urlNull ? null : artist;

		return new(type, yearFilter, monthFilter, artistFilter);
	}

	public string ClearFilter(Enums.MenuButtonsType type)
	{
		// TODO custom enum
		return type switch
		{
			Enums.MenuButtonsType.Date => GetFilterUrl(null, null),
			Enums.MenuButtonsType.Artists => GetFilterUrl(artist: null),
			_ => throw new NotSupportedException(nameof(Enums.MenuButtonsType)),
		};
	}
}
