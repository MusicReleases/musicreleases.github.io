using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyReleaseEntity : SpotifyIdNameEntity
{
	public DateTime ReleaseDate { get; init; }
	public int TotalTracks { get; init; }

	public string? UrlApp { get; init; }
	public string? UrlWeb { get; init; }
	public string? UrlImage { get; init; }


	//public List<Image> Images { get; init; }
	//public HashSet<SpotifyArtist> Artists { get; init; }
	//public SortedSet<SpotifyTrack>? Tracks { get; init; }

	public ReleaseType ReleaseType { get; init; }

	public SpotifyReleaseEntity() { }

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
