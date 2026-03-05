using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyArtistReleaseEntityOld : SpotifyIdEntity
{
	public required string ArtistId { get; init; }
	public required string ReleaseId { get; init; }

	public SpotifyArtistReleaseEntityOld() { }

	[SetsRequiredMembers]
	public SpotifyArtistReleaseEntityOld(string artistId, string releaseId)
	{
		ArtistId = artistId;
		ReleaseId = releaseId;
	}
}
