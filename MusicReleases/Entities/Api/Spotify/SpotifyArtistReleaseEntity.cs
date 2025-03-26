using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyArtistReleaseEntity : SpotifyIdEntity
{
	public required string ArtistId { get; init; }
	public required string ReleaseId { get; init; }

	public SpotifyArtistReleaseEntity() { }

	[SetsRequiredMembers]
	public SpotifyArtistReleaseEntity(string artistId, string releaseId)
	{
		ArtistId = artistId;
		ReleaseId = releaseId;
	}
}
