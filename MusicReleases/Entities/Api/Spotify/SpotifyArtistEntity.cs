using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyArtistEntity : SpotifyIdNameEntity
{
	public bool Following { get; set; }
}
