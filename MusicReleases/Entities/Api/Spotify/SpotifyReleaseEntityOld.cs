using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyReleaseEntityOld : SpotifyIdNameUrlEntity
{
	public DateTime ReleaseDate { get; init; }
	public int TotalTracks { get; init; }

	public required string UrlImage { get; init; }


	//public List<Image> Images { get; init; }
	//public HashSet<SpotifyArtist> Artists { get; init; }
	//public SortedSet<SpotifyTrack>? Tracks { get; init; }

	public MainReleasesType ReleaseType { get; init; }

	public SpotifyReleaseEntityOld() { }

	[SetsRequiredMembers]
	public SpotifyReleaseEntityOld(SpotifyReleaseOld spotifyRelease)
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
