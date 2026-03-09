using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Objects.Spotify;

public class SpotifyReleaseFilter
{
	public MainReleasesType ReleaseType { get; set; } = MainReleasesType.Albums;

	public ReleaseAdvancedFilter ReleaseAdvancedFilter { get; set; } = ReleaseAdvancedFilter.All;

	public string? Artist { get; set; }

	public int? Year { get; set; }

	public DateTime? Month { get; set; }

	public string? SearchText { get; set; }

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
}
