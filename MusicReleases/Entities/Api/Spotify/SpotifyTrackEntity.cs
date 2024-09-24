using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyTrackEntity : SpotifyIdNameEntity
{
	public string? AlbumId { get; set; }
}
