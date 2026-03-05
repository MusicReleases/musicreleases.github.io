using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Services;

public interface ISpotifyApiReleaseService
{
	Task<SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>?> GetReleases(MainReleasesType releaseType, ISet<SpotifyArtist> artists, SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>? existingReleases = null, bool forceUpdate = false);
	public static DateTime? GetLastTimeUpdate(SpotifyUserListUpdateRelease lastUpdateList, MainReleasesType releaseType) => releaseType switch
	{
		MainReleasesType.Albums => lastUpdateList.LastUpdateAlbums,
		MainReleasesType.Tracks => lastUpdateList.LastUpdateTracks,
		MainReleasesType.Appears => lastUpdateList.LastUpdateAppears,
		MainReleasesType.Compilations => lastUpdateList.LastUpdateCompilations,
		MainReleasesType.Podcasts => lastUpdateList.LastUpdatePodcasts,
		_ => throw new NotSupportedException(nameof(releaseType)),
	};
}