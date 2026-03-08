using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Objects;

public class SpotifyReleaseFilter
{
	public MainReleasesType ReleaseType { get; set; } = MainReleasesType.Albums;

	public ReleaseAdvancedFilter ReleaseAdvancedFilter { get; set; } = ReleaseAdvancedFilter.All;

	public string? Artist { get; set; }

	public int? Year { get; set; }

	public DateTime? Month { get; set; }

	public string? SearchText { get; set; }

	public SpotifyFilterAdvanced Advanced { get; init; } = new();

	public SpotifyReleaseFilter()
	{

	}

	public SpotifyReleaseFilter(MainReleasesType releaseType)
	{
		ReleaseType = releaseType;
	}

	public SpotifyReleaseFilter(MainReleasesType releaseType, ReleaseAdvancedFilter advancedFilter, string? artist, int? year, DateTime? month, string? searchText = null)
	{
		ReleaseType = releaseType;
		ReleaseAdvancedFilter = advancedFilter;
		Artist = artist;
		Year = year;
		Month = month;
		SearchText = searchText;
	}

	public SpotifyReleaseFilter(MainReleasesType releaseType, int? year, DateTime? month, string? artist, SpotifyFilterAdvanced advancedFilter)
	{
		ReleaseType = releaseType;
		Artist = artist;
		Year = year;
		Month = month;
		Advanced = advancedFilter;
	}
	public SpotifyReleaseFilter(SpotifyFilterEntityOld filterDn, SpotifyFilterAdvanced advancedFilter)
	{
		ReleaseType = filterDn.ReleaseType;
		Artist = filterDn.Artist;
		Year = filterDn.Year;
		Month = filterDn.Month;
		Advanced = advancedFilter;
	}
}
