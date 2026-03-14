using JakubKastner.SpotifyApi.Objects.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyPlaylist : SpotifyIdNameUrlObject, IComparable
{
	public required string SnapshotId { get; set; }

	public required string OwnerId { get; init; }

	public required bool Collaborative { get; init; }


	public HashSet<string> Tracks { get; set; } = [];


	public SpotifyPlaylist()
	{
		// ctor for json
	}

	[SetsRequiredMembers]
	public SpotifyPlaylist(string id, string name, string urlApp, string urlWeb, string snapshotId, string ownerId, bool collaborative) : base(id, name, urlApp, urlWeb)
	{
		SnapshotId = snapshotId;
		OwnerId = ownerId;
		Collaborative = collaborative;
	}

	public void AddTracks(string snapshotId, ISet<string> trackIds)
	{
		SnapshotId = snapshotId;
		Tracks.UnionWith(trackIds);
	}

	public void RemoveTracks(string snapshotId, ISet<string> trackIds)
	{
		SnapshotId = snapshotId;
		Tracks.ExceptWith(trackIds);
	}
}
