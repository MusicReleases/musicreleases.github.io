using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyPlaylist : SpotifyIdNameObject, IComparable
{
	public required bool CurrentUserOwned { get; init; }
	public required bool Collaborative { get; init; }
	public string? SnapshotId { get; init; }

	public int? TotalTracks { get; init; }
	public HashSet<string> Tracks { get; set; } = [];

	// TODO playlist owner - currentuserowned

	public SpotifyPlaylist()
	{
		// TODO ctor for json
	}

	[SetsRequiredMembers]
	public SpotifyPlaylist(FullPlaylist fullPlaylist, HashSet<string> tracks, bool currentUserOwned = false)
	{
		// TODO null
		Id = fullPlaylist.Id ?? "";
		Name = fullPlaylist.Name ?? "";
		TotalTracks = fullPlaylist.Tracks?.Total;
		Collaborative = fullPlaylist.Collaborative ?? false;
		CurrentUserOwned = currentUserOwned;
		SnapshotId = fullPlaylist.SnapshotId;
		Tracks = tracks;
	}
}
