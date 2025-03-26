using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyArtistEntity : SpotifyIdNameEntity
{
	public required bool Following { get; init; }

	public SpotifyArtistEntity() { }

	[SetsRequiredMembers]
	public SpotifyArtistEntity(SpotifyArtist spotifyArtist, bool following)
	{
		Id = spotifyArtist.Id;
		Name = spotifyArtist.Name;
		Following = following;
	}
}
