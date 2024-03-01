using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public interface ISpotifyReleasesController
{
	bool Loading { get; }
	SortedSet<SpotifyRelease>? Releases { get; }

	void LoadReleases(SpotifyEnums.ReleaseType releaseType);
	void SaveReleasesToStorage();
}