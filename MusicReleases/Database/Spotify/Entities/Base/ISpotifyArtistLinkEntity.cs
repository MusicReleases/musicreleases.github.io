using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Database.Spotify.Entities.Base;

internal interface ISpotifyArtistLinkEntity
{
	string ArtistId { get; init; }

	string ReleaseId { get; init; }

	ArtistReleaseRole Role { get; init; }

	ReleaseType ReleaseType { get; init; }
}
