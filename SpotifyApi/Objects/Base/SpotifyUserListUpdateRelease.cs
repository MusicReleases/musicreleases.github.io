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

	public SpotifyUserListUpdateRelease(DateTime lastUpdate, MainReleasesType releaseType)
	{
		switch (releaseType)
		{
			case MainReleasesType.Albums:
				LastUpdateAlbums = lastUpdate;
				break;
			case MainReleasesType.Tracks:
				LastUpdateTracks = lastUpdate;
				break;
			case MainReleasesType.Appears:
				LastUpdateAppears = lastUpdate;
				break;
			case MainReleasesType.Compilations:
				LastUpdateCompilations = lastUpdate;
				break;
			case MainReleasesType.Podcasts:
				LastUpdatePodcasts = lastUpdate;
				break;
			default:
				throw new NotSupportedException(nameof(releaseType));
		}
	}
}
