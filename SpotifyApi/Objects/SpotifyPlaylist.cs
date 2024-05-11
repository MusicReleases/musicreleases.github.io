using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyPlaylist : SpotifyIdObject, IComparable
{
	public bool CurrentUserOwned { get; private set; }
	public bool Collaborative { get; private set; }
	public string? SnapshotId { get; private set; }

	public int? TotalTracks { get; private set; }
	public HashSet<SpotifyTrack> Tracks { get; set; } = [];

	// TODO playlist owner - currentuserowned
	public SpotifyPlaylist(FullPlaylist fullPlaylist, bool currentUserOwned = false) : base(fullPlaylist.Id ?? "", fullPlaylist.Name ?? "")
	{
		// TODO null
		TotalTracks = fullPlaylist.Tracks?.Total;
		Collaborative = fullPlaylist.Collaborative ?? false;
		CurrentUserOwned = currentUserOwned;
		SnapshotId = fullPlaylist.SnapshotId;
	}
}
