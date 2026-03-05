
using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyUserArtistEntityOld : SpotifyIdEntity
{
	public required string UserId { get; set; }

	public required string ArtistId { get; set; }

	public SpotifyUserArtistEntityOld()
	{ }

	[SetsRequiredMembers]
	public SpotifyUserArtistEntityOld(string userId, string artistId)
	{
		UserId = userId;
		ArtistId = artistId;
	}
}
