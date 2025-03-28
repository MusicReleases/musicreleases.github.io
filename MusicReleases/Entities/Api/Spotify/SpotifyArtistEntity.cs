using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyArtistEntity : SpotifyIdNameEntity
{
	public SpotifyArtistEntity() { }

	[SetsRequiredMembers]
	public SpotifyArtistEntity(SpotifyArtist spotifyArtist)
	{
		Id = spotifyArtist.Id;
		Name = spotifyArtist.Name;
	}
}
