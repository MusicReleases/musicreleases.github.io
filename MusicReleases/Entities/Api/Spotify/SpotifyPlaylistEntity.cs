using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyPlaylistEntity : SpotifyIdNameUrlEntity
{
	public bool Collaborative { get; init; }
	public bool CurrentUserOwned { get; init; }
	public string? SnapshotId { get; init; }

	public SpotifyPlaylistEntity() { }

	[SetsRequiredMembers]
	public SpotifyPlaylistEntity(SpotifyPlaylist spotifyPlaylist)
	{
		Id = spotifyPlaylist.Id;
		Name = spotifyPlaylist.Name;
		Collaborative = spotifyPlaylist.Collaborative;
		CurrentUserOwned = spotifyPlaylist.CurrentUserOwned;
		UrlApp = spotifyPlaylist.UrlApp;
		UrlWeb = spotifyPlaylist.UrlWeb;
	}
}
