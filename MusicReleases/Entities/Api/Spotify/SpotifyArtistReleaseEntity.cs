using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyArtistReleaseEntity : SpotifyGuidEntity
{
	public string? ArtistId { get; set; }
	public string? ReleaseId { get; set; }

	public SpotifyArtistReleaseEntity() { }

	public SpotifyArtistReleaseEntity(string artistId, string releaseId)
	{
		ArtistId = artistId;
		ReleaseId = releaseId;
	}
}
