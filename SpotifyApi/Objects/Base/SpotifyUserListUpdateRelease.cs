using JakubKastner.SpotifyApi.SpotifyEnums;

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

	public SpotifyUserListUpdateRelease(DateTime lastUpdate, ReleaseGroup releaseType)
	{
		switch (releaseType)
		{
			case ReleaseGroup.Albums:
				LastUpdateAlbums = lastUpdate;
				break;
			case ReleaseGroup.Tracks:
				LastUpdateTracks = lastUpdate;
				break;
			case ReleaseGroup.Appears:
				LastUpdateAppears = lastUpdate;
				break;
			case ReleaseGroup.Compilations:
				LastUpdateCompilations = lastUpdate;
				break;
			case ReleaseGroup.Podcasts:
				LastUpdatePodcasts = lastUpdate;
				break;
			default:
				throw new NotSupportedException(nameof(releaseType));
		}
	}
}
