using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Objects.Base;

public class SpotifyUserListUpdateRelease : SpotifyUserListUpdate
{
	public DateTime? LastUpdateAlbums { get; set; }
	public DateTime? LastUpdateTracks { get; set; }
	public DateTime? LastUpdateAppears { get; set; }
	public DateTime? LastUpdateCompilations { get; set; }
	public DateTime? LastUpdatePodcasts { get; set; }

	public SpotifyUserListUpdateRelease()
	{ }

	public SpotifyUserListUpdateRelease(DateTime lastUpdate, ReleaseType releaseType)
	{
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
