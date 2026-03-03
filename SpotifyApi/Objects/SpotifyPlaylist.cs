using JakubKastner.SpotifyApi.Objects.Base;
using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyPlaylist : SpotifyIdNameUrlObject, IComparable
{
	public required bool CurrentUserOwned { get; init; }
	public required bool Collaborative { get; init; }
	public required string SnapshotId { get; set; }


	public required string OwnerId { get; init; }

	public int? TotalItems { get; init; }
	// TODO private set
	public HashSet<string> Tracks { get; set; } = [];

	// TODO playlist owner - currentuserowned

	public SpotifyPlaylist()
	{
		// TODO ctor for json
	}

	[SetsRequiredMembers]
	public SpotifyPlaylist(FullPlaylist fullPlaylist)
	{
		// TODO null
		Id = fullPlaylist.Id ?? "";
		Name = fullPlaylist.Name ?? "";
		TotalItems = fullPlaylist.Items?.Total;
		Collaborative = fullPlaylist.Collaborative ?? false;
		//CurrentUserOwned = currentUserOwned;
		SnapshotId = fullPlaylist.SnapshotId ?? "";
		UrlApp = fullPlaylist.Uri ?? "";
		UrlWeb = fullPlaylist.ExternalUrls?["spotify"] ?? "";
		//Tracks = [];

		OwnerId = fullPlaylist.Owner?.Id ?? "";
	}

	[SetsRequiredMembers]
	public SpotifyPlaylist(FullPlaylist fullPlaylist, HashSet<string> tracks, bool currentUserOwned = false)
	{
		// TODO null
		Id = fullPlaylist.Id ?? "";
		Name = fullPlaylist.Name ?? "";
		TotalItems = fullPlaylist.Items?.Total;
		Collaborative = fullPlaylist.Collaborative ?? false;
		CurrentUserOwned = currentUserOwned;
		SnapshotId = fullPlaylist.SnapshotId ?? "";
		UrlApp = fullPlaylist.Uri ?? "";
		UrlWeb = fullPlaylist.ExternalUrls?["spotify"] ?? "";
		Tracks = tracks;

		OwnerId = "";
	}

	[SetsRequiredMembers]
	public SpotifyPlaylist(string id, string name, string urlApp, string urlWeb, string snapshotId, string ownerId, bool collaborative)
	{
		Id = id;
		Name = name;
		UrlApp = urlApp;
		UrlWeb = urlWeb;
		SnapshotId = snapshotId;
		OwnerId = ownerId;
		Collaborative = collaborative;

		// TODO CurrentUserOwned
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
