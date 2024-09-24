using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyArtistEntity : SpotifyIdNameEntity
{
	public bool Following { get; set; }

	public SpotifyArtistEntity() { }

	public SpotifyArtistEntity(SpotifyArtist spotifyArtist, bool following)
	{
		Id = spotifyArtist.Id;
		Name = spotifyArtist.Name;
		Following = following;
	}
}
