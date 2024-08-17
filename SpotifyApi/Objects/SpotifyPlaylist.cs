using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyPlaylist : SpotifyIdNameObject, IComparable
{
	public bool CurrentUserOwned { get; init; }
	public bool Collaborative { get; init; }
	public string? SnapshotId { get; init; }

	public int? TotalTracks { get; init; }
	public HashSet<string> Tracks { get; set; } = [];

	// TODO playlist owner - currentuserowned

	public SpotifyPlaylist() : base("json", "init")
	{
		// TODO ctor for json
	}

	public SpotifyPlaylist(FullPlaylist fullPlaylist, HashSet<string> tracks, bool currentUserOwned = false) : base(fullPlaylist.Id ?? "", fullPlaylist.Name ?? "")
	{
		// TODO null
		TotalTracks = fullPlaylist.Tracks?.Total;
		Collaborative = fullPlaylist.Collaborative ?? false;
		CurrentUserOwned = currentUserOwned;
		SnapshotId = fullPlaylist.SnapshotId;
		Tracks = tracks;
	}
}
