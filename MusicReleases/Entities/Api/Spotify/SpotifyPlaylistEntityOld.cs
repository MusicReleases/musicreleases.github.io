using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyPlaylistEntityOld : SpotifyIdNameUrlEntity
{
	public bool Collaborative { get; init; }
	public bool CurrentUserOwned { get; init; }
	public required string SnapshotId { get; init; }

	public SpotifyPlaylistEntityOld() { }

	[SetsRequiredMembers]
	public SpotifyPlaylistEntityOld(SpotifyPlaylist spotifyPlaylist)
	{
		Id = spotifyPlaylist.Id;
		Name = spotifyPlaylist.Name;
		Collaborative = spotifyPlaylist.Collaborative;
		CurrentUserOwned = spotifyPlaylist.CurrentUserOwned;
		SnapshotId = spotifyPlaylist.SnapshotId;
		UrlApp = spotifyPlaylist.UrlApp;
		UrlWeb = spotifyPlaylist.UrlWeb;
	}
}
