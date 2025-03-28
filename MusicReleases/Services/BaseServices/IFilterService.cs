using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.Base;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface IFilterService
{
	SpotifyFilter SpotifyFilter { get; }
	void FilterReleaseType(SpotifyEnums.ReleaseType releaseType);
}