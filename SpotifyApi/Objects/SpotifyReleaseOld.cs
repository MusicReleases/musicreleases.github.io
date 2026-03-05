using JakubKastner.Extensions;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.SpotifyEnums;
using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyReleaseOld : SpotifyIdNameUrlObject, IComparable
{
	public MainReleasesType ReleaseType { get; init; }

	public required DateTime ReleaseDate { get; init; }

	public required string UrlImage { get; init; }

	public required int TotalTracks { get; init; }

	public bool New { get; init; } = false;


	public HashSet<string>? ArtistIds { get; init; }

	//public List<Image> Images { get; init; }

	// TODO not null - after implementation of saving to db
	public HashSet<SpotifyArtist>? Artists { get; init; }

	public SortedSet<SpotifyTrack>? Tracks { get; set; }

	// TODO artists - GetArtists
	// TODO images (0), default
	public SpotifyReleaseOld()
	{
		// TODO ctor for json
	}

	[SetsRequiredMembers]
	public SpotifyReleaseOld(string id, string name, MainReleasesType releaseType, DateTime releaseDate, string urlApp, string urlWeb, string urlImage, int totalTracks)
	{
		Id = id;
		Name = name;
		ReleaseType = releaseType;
		ReleaseDate = releaseDate;
		UrlApp = urlApp;
		UrlWeb = urlWeb;
		UrlImage = urlImage;
		TotalTracks = totalTracks;
		New = true;

		// TODO artists
	}

	[SetsRequiredMembers]
	public SpotifyReleaseOld(SimpleAlbum simpleAlbum, MainReleasesType releaseType)
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
		ArtistIds = simpleAlbum.Artists.Select(a => a.Id).ToHashSet();
		ReleaseType = releaseType;
		New = true;
	}

	[SetsRequiredMembers]
	public SpotifyReleaseOld(FullAlbum fullAlbum, MainReleasesType releaseType)
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
		ArtistIds = fullAlbum.Artists.Select(a => a.Id).ToHashSet();
		ReleaseType = releaseType;
		New = true;
	}

	[SetsRequiredMembers]
	public SpotifyReleaseOld(SimpleShow simpleShow)
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
		ArtistIds = ["0"];
		ReleaseType = MainReleasesType.Podcasts;
		New = true;
	}

	public new int CompareTo(object? obj)
	{
		if (obj == null)
		{
			return -1;
		}

		var other = (SpotifyReleaseOld)obj;
		var dateComparison = other.ReleaseDate.CompareTo(ReleaseDate);

		return (dateComparison != 0) ? dateComparison : Name.CompareTo(other.Name);
	}
}
