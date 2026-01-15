using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyReleaseEntity : SpotifyIdNameUrlEntity
{
	public DateTime ReleaseDate { get; init; }
	public int TotalTracks { get; init; }

	public required string UrlImage { get; init; }


	//public List<Image> Images { get; init; }
	//public HashSet<SpotifyArtist> Artists { get; init; }
	//public SortedSet<SpotifyTrack>? Tracks { get; init; }

	public ReleaseType ReleaseType { get; init; }

	public SpotifyReleaseEntity() { }

	[SetsRequiredMembers]
	public SpotifyReleaseEntity(SpotifyRelease spotifyRelease)
	{
		Id = spotifyRelease.Id;
		Name = spotifyRelease.Name;
		ReleaseDate = spotifyRelease.ReleaseDate;
		TotalTracks = spotifyRelease.TotalTracks;
		UrlApp = spotifyRelease.UrlApp;
		UrlWeb = spotifyRelease.UrlWeb;
		UrlImage = spotifyRelease.UrlImage;
		ReleaseType = spotifyRelease.ReleaseType;
	}
}
