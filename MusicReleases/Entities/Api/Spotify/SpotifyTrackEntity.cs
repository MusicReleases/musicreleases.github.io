using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyTrackEntity : SpotifyIdNameUrlEntity
{
	public required string AlbumId { get; set; }
}
