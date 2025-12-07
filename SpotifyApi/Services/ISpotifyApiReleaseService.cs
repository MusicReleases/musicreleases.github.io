using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Services;

public interface ISpotifyApiReleaseService
{
	Task<SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>?> GetReleases(ReleaseType releaseType, ISet<SpotifyArtist> artists, SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? existingReleases = null, bool forceUpdate = false);
	public static DateTime? GetLastTimeUpdate(SpotifyUserListUpdateRelease lastUpdateList, ReleaseType releaseType) => releaseType switch
	{
		ReleaseType.Albums => lastUpdateList.LastUpdateAlbums,
		ReleaseType.Tracks => lastUpdateList.LastUpdateTracks,
		ReleaseType.Appears => lastUpdateList.LastUpdateAppears,
		ReleaseType.Compilations => lastUpdateList.LastUpdateCompilations,
		ReleaseType.Podcasts => lastUpdateList.LastUpdatePodcasts,
		_ => throw new NotSupportedException(nameof(releaseType)),
	};
}