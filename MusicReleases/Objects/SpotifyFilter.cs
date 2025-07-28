using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Objects;

public class SpotifyFilter
{
	public ReleaseType ReleaseType { get; init; } = ReleaseType.Albums;
	public string? Artist { get; init; }
	public int? Year { get; init; }
	public DateTime? Month { get; init; }
	public SpotifyFilterAdvanced Advanced { get; init; } = new();


	public SpotifyFilter()
	{

	}
	public SpotifyFilter(SpotifyFilter filter)
	{
		ReleaseType = filter.ReleaseType;
		Artist = filter.Artist;
		Year = filter.Year;
		Month = filter.Month;
		Advanced = filter.Advanced;
	}
	public SpotifyFilter(ReleaseType releaseType, int? year, DateTime? month, string? artist, SpotifyFilterAdvanced advancedFilter)
	{
		ReleaseType = releaseType;
		Artist = artist;
		Year = year;
		Month = month;
		Advanced = advancedFilter;
	}
}
