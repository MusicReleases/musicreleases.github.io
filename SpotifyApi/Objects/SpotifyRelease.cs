using JakubKastner.SpotifyApi.Objects.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyRelease : SpotifyIdNameUrlObject, IComparable
{
	public ReleaseType ReleaseType { get; init; }

	public required DateTime ReleaseDate { get; init; }

	public required string UrlImage { get; init; }

	public required int TotalTracks { get; init; }

	public required bool New { get; init; }

	public required HashSet<SpotifyArtist> Artists { get; init; }

	public required HashSet<SpotifyArtist> FeaturedArtists { get; init; }

	public SortedSet<SpotifyTrack>? Tracks { get; set; }

	public SpotifyRelease()
	{
		// ctor for json
	}

	[SetsRequiredMembers]
	public SpotifyRelease(string id, string name, string urlApp, string urlWeb, ReleaseType releaseType, DateTime releaseDate, string urlImage, int totalTracks, bool isNew, HashSet<SpotifyArtist> artists, HashSet<SpotifyArtist> featuredArtists) : base(id, name, urlApp, urlWeb)
	{
		ReleaseType = releaseType;
		ReleaseDate = releaseDate;
		UrlImage = urlImage;
		TotalTracks = totalTracks;
		New = isNew;
		Artists = artists;
		FeaturedArtists = featuredArtists;
	}

	public new int CompareTo(object? obj)
	{
		if (obj == null)
		{
			return -1;
		}

		var other = (SpotifyRelease)obj;
		var dateComparison = other.ReleaseDate.CompareTo(ReleaseDate);

		return (dateComparison != 0) ? dateComparison : Name.CompareTo(other.Name);
	}
}
