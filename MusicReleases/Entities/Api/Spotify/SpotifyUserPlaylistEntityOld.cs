
using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyUserPlaylistEntityOld : SpotifyIdEntity
{
	public required string UserId { get; set; }

	public required string PlaylistId { get; set; }

	public SpotifyUserPlaylistEntityOld()
	{ }

	[SetsRequiredMembers]
	public SpotifyUserPlaylistEntityOld(string userId, string playlistId)
	{
		UserId = userId;
		PlaylistId = playlistId;
	}
}
