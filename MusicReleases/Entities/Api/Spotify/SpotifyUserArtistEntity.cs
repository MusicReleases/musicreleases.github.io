
using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyUserArtistEntity : SpotifyIdEntity
{
	public required string UserId { get; set; }

	public required string ArtistId { get; set; }

	public SpotifyUserArtistEntity()
	{ }

	[SetsRequiredMembers]
	public SpotifyUserArtistEntity(string userId, string artistId)
	{
		UserId = userId;
		ArtistId = artistId;
	}
}
