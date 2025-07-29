using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;
using JakubKastner.MusicReleases.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify.User;

public class SpotifyFilterEntity : SpotifyIdEntity
{
	public ReleaseType ReleaseType { get; init; }
	public string? Artist { get; init; }
	public int? Year { get; init; }
	public DateTime? Month { get; init; }
	public bool Tracks { get; init; }
	public bool EPs { get; init; }
	public bool NotRemixes { get; init; }
	public bool Remixes { get; init; }
	public bool FollowedArtists { get; init; }
	public bool SavedReleases { get; init; }
	public bool NotVariousArtists { get; init; }
	public bool VariousArtists { get; init; }
	public bool NewReleases { get; init; }
	public bool OldReleases { get; init; }

	public SpotifyFilterEntity()
	{ }

	public SpotifyFilterEntity(SpotifyFilter filter, string userId)
	{
		Id = userId;
		ReleaseType = filter.ReleaseType;
		Artist = filter.Artist;
		Year = filter.Year;
		Month = filter.Month;
		Tracks = filter.Advanced.Tracks;
		EPs = filter.Advanced.EPs;
		NotRemixes = filter.Advanced.NotRemixes;
		Remixes = filter.Advanced.Remixes;
		FollowedArtists = filter.Advanced.FollowedArtists;
		SavedReleases = filter.Advanced.SavedReleases;
		NotVariousArtists = filter.Advanced.NotVariousArtists;
		VariousArtists = filter.Advanced.VariousArtists;
		NewReleases = filter.Advanced.NewReleases;
		OldReleases = filter.Advanced.OldReleases;
	}
}
