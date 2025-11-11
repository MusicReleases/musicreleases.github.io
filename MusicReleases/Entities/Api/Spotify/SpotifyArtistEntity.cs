using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyArtistEntity : SpotifyIdNameUrlEntity
{
	public SpotifyArtistEntity() { }

	[SetsRequiredMembers]
	public SpotifyArtistEntity(SpotifyArtist spotifyArtist)
	{
		Id = spotifyArtist.Id;
		Name = spotifyArtist.Name;
		UrlApp = spotifyArtist.UrlApp;
		UrlWeb = spotifyArtist.UrlWeb;
	}
}
