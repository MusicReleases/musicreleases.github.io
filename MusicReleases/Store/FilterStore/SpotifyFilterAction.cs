using JakubKastner.MusicReleases.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Store.FilterStore;

public class SpotifyFilterAction()
{
	public record SetReleaseTypeFilterAction(ReleaseType ReleaseType);

	public record SetArtistFilterAction(string? ArtistId);

	public record SetYearFilterAction(int? Year);

	public class SetMonthFilterAction(int? year, int? month)
	{
		public DateTime? Month { get; } = year.HasValue && month.HasValue ? new(year.Value, month.Value, 1) : null;
	}

	public record ResetFiltersAction;
	public record SetFiltersAction(SpotifyFilter Filter);
}
