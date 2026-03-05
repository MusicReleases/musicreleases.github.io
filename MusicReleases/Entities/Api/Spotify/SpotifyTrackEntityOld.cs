using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyTrackEntityOld : SpotifyIdNameUrlEntity
{
	public required string AlbumId { get; set; }
}
