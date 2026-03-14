using JakubKastner.SpotifyApi.Objects.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

[method: SetsRequiredMembers]
public class SpotifyPlaylist(string id, string name, string urlApp, string urlWeb, string snapshotId, string ownerId, bool collaborative) : SpotifyIdNameUrlObject(id, name, urlApp, urlWeb), IComparable
{
	public required string SnapshotId { get; set; } = snapshotId;

	public required string OwnerId { get; init; } = ownerId;

	public required bool Collaborative { get; init; } = collaborative;


	public HashSet<string> Tracks { get; set; } = [];

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
