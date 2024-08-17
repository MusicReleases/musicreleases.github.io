using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserListReleases<T> : SpotifyUserList<T> where T : SpotifyIdNameObject
{
	public DateTime LastUpdateAlbums { get; init; }
	public DateTime LastUpdateTracks { get; init; }
	public DateTime LastUpdateAppears { get; init; }
	public DateTime LastUpdateCompilations { get; init; }
	public DateTime LastUpdatePodcasts { get; init; }

	public SpotifyUserListReleases(ISet<T> list, DateTime lastUpdate, ReleaseType releaseType)
	{
		List = list;

		// update last update
		switch (releaseType)
		{
			case ReleaseType.Albums:
				LastUpdateAlbums = lastUpdate;
				break;
			case ReleaseType.Tracks:
				LastUpdateTracks = lastUpdate;
				break;
			case ReleaseType.Appears:
				LastUpdateAppears = lastUpdate;
				break;
			case ReleaseType.Compilations:
				LastUpdateCompilations = lastUpdate;
				break;
			case ReleaseType.Podcasts:
				LastUpdatePodcasts = lastUpdate;
				break;
			default:
				throw new NotSupportedException(nameof(releaseType));
		}
	}
}