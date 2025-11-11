using JakubKastner.SpotifyApi.Objects.Base;
using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyPlaylist : SpotifyIdNameUrlObject, IComparable
{
	public required bool CurrentUserOwned { get; init; }
	public required bool Collaborative { get; init; }
	public required string SnapshotId { get; set; }

	public int? TotalTracks { get; init; }
	// TODO private set
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
		SnapshotId = fullPlaylist.SnapshotId ?? "";
		UrlApp = fullPlaylist.Uri ?? "";
		UrlWeb = fullPlaylist.Href ?? "";
		Tracks = tracks;
	}

	public void AddTracks(string snapshotId, ISet<string> trackIds)
	{
		SnapshotId = snapshotId;
		Tracks.UnionWith(trackIds);
	}
}
