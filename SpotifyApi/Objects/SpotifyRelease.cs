using JakubKastner.Extensions;
using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects.Base;
using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyRelease : SpotifyIdNameUrlObject, IComparable
{
	public ReleaseType ReleaseType { get; init; }

	public required DateTime ReleaseDate { get; init; }

	public required string UrlImage { get; init; }

	public required int TotalTracks { get; init; }

	public bool New { get; init; } = false;

	//public List<Image> Images { get; init; }

	// TODO not null - after implementation of saving to db
	public required HashSet<SpotifyArtist> Artists { get; init; }

	public required HashSet<SpotifyArtist> FeaturedArtists { get; init; }

	public SortedSet<SpotifyTrack>? Tracks { get; set; }

	// TODO artists - GetArtists
	// TODO images (0), default
	public SpotifyRelease()
	{
		// TODO ctor for json
	}

	[SetsRequiredMembers]
	public SpotifyRelease(string id, string name, ReleaseType releaseType, DateTime releaseDate, string urlApp, string urlWeb, string urlImage, int totalTracks, HashSet<SpotifyArtist> artists, HashSet<SpotifyArtist> featuredArtists)
	{
		Id = id;
		Name = name;
		ReleaseType = releaseType;
		ReleaseDate = releaseDate;
		UrlApp = urlApp;
		UrlWeb = urlWeb;
		UrlImage = urlImage;
		TotalTracks = totalTracks;
		New = false; // from db
		Artists = artists;
		FeaturedArtists = featuredArtists;
	}

	[SetsRequiredMembers]
	public SpotifyRelease(SimpleAlbum simpleAlbum, HashSet<SpotifyArtist> featuredArtists)
	{
		Id = simpleAlbum.Id;
		Name = simpleAlbum.Name;
		ReleaseDate = simpleAlbum.ReleaseDate.ToDateTimeNullable() ?? new(1900, 1, 1);
		TotalTracks = simpleAlbum.TotalTracks;

		//Images = simpleAlbum.Images;
		if (simpleAlbum.Images.Count > 0)
		{
			if (simpleAlbum.Images[1] is not null)
			{
				UrlImage = simpleAlbum.Images[1].Url;
			}
			else
			{
				UrlImage = simpleAlbum.Images.First().Url;
			}
		}
		else
		{
			UrlImage = "";
		}
		UrlApp = simpleAlbum.Uri;
		UrlWeb = simpleAlbum.ExternalUrls["spotify"];
		Artists = simpleAlbum.Artists.Select(simpleArtist => new SpotifyArtist(simpleArtist)).ToHashSet();
		FeaturedArtists = featuredArtists;
		ReleaseType = MapReleaseTypeFromApi(simpleAlbum.AlbumType);
		New = true;
	}

	[SetsRequiredMembers]
	public SpotifyRelease(FullAlbum fullAlbum, HashSet<SpotifyArtist> featuredArtists)
	{
		Id = fullAlbum.Id;
		Name = fullAlbum.Name;
		ReleaseDate = fullAlbum.ReleaseDate.ToDateTimeNullable() ?? new(1900, 1, 1);
		TotalTracks = fullAlbum.TotalTracks;
		//Images = fullAlbum.Images;
		if (fullAlbum.Images.Count > 0)
		{
			if (fullAlbum.Images[1] is not null)
			{
				UrlImage = fullAlbum.Images[1].Url;
			}
			else
			{
				UrlImage = fullAlbum.Images.First().Url;
			}
		}
		else
		{
			UrlImage = "";
		}
		UrlApp = fullAlbum.Uri;
		UrlWeb = fullAlbum.ExternalUrls["spotify"];
		Artists = fullAlbum.Artists.Select(simpleArtist => new SpotifyArtist(simpleArtist)).ToHashSet();
		FeaturedArtists = featuredArtists;
		ReleaseType = MapReleaseTypeFromApi(fullAlbum.AlbumType);
		New = true;
	}

	[SetsRequiredMembers]
	public SpotifyRelease(SimpleShow simpleShow)
	{
		Id = simpleShow.Id;
		Name = simpleShow.Name;
		// TODO podcast release date
		//ReleaseDate = null; 
		TotalTracks = 1;
		//Images = simpleShow.Images;
		if (simpleShow.Images.Count > 0)
		{
			if (simpleShow.Images[1] is not null)
			{
				UrlImage = simpleShow.Images[1].Url;
			}
			else
			{
				UrlImage = simpleShow.Images.First().Url;
			}
		}
		else
		{
			UrlImage = "";
		}
		UrlApp = simpleShow.Uri;
		UrlWeb = simpleShow.ExternalUrls["spotify"];
		// TODO artist for simple show
		Artists =
		[
			new("0", "Podcast", "", "")
		];
		FeaturedArtists = [];
		ReleaseType = ReleaseType.Podcast;
		New = true;
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

	private static ReleaseType MapReleaseTypeFromApi(string releaseTypeApi) => releaseTypeApi switch
	{
		"album" => ReleaseType.Album,
		"single" => ReleaseType.Track,
		"compilation" => ReleaseType.Compilation,
		_ => throw new ArgumentOutOfRangeException(nameof(releaseTypeApi), $"Not expected release type value: {releaseTypeApi}"),
	};
}
