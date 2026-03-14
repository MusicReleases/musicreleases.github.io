using JakubKastner.SpotifyApi.Objects.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

[method: SetsRequiredMembers]
public class SpotifyRelease(string id, string name, string urlApp, string urlWeb, ReleaseType releaseType, DateTime releaseDate, string urlImage, int totalTracks, bool isNew, HashSet<SpotifyArtist> artists, HashSet<SpotifyArtist> featuredArtists) : SpotifyIdNameUrlObject(id, name, urlApp, urlWeb), IComparable
{
	public ReleaseType ReleaseType { get; init; } = releaseType;

	public required DateTime ReleaseDate { get; init; } = releaseDate;

	public required string UrlImage { get; init; } = urlImage;

	public required int TotalTracks { get; init; } = totalTracks;

	public required bool New { get; init; } = isNew;

	public required HashSet<SpotifyArtist> Artists { get; init; } = artists;

	public required HashSet<SpotifyArtist> FeaturedArtists { get; init; } = featuredArtists;

	public SortedSet<SpotifyTrack>? Tracks { get; set; }

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
