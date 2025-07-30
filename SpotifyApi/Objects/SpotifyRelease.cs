using JakubKastner.Extensions;
using JakubKastner.SpotifyApi.Objects.Base;
using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyRelease : SpotifyIdNameObject, IComparable
{
	public required DateTime ReleaseDate { get; init; }
	public required int TotalTracks { get; init; }
	public bool New { get; init; } = false;

	public required string UrlApp { get; init; }
	public required string UrlWeb { get; init; }
	public required string UrlImage { get; init; }

	public string ArtistString => string.Join(", ", Artists.Select(x => x.Name));
	public string ReleaseDateString => ReleaseDate.ToString("dd.MM.yyyy");

	//public List<Image> Images { get; init; }

	// TODO not null - after implementation of saving to db
	public required HashSet<SpotifyArtist> Artists { get; init; }

	public SortedSet<SpotifyTrack>? Tracks { get; set; }

	public ReleaseType ReleaseType { get; init; }

	// TODO artists - GetArtists
	// TODO images (0), default
	public SpotifyRelease()
	{
		// TODO ctor for json
	}


	[SetsRequiredMembers]
	public SpotifyRelease(SimpleAlbum simpleAlbum, ReleaseType releaseType)
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
		UrlWeb = simpleAlbum.Href;
		Artists = simpleAlbum.Artists.Select(simpleArtist => new SpotifyArtist(simpleArtist)).ToHashSet();
		ReleaseType = releaseType;
		New = true;
	}

	[SetsRequiredMembers]
	public SpotifyRelease(FullAlbum fullAlbum, ReleaseType releaseType)
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
		UrlWeb = fullAlbum.Href;
		Artists = fullAlbum.Artists.Select(simpleArtist => new SpotifyArtist(simpleArtist)).ToHashSet();
		ReleaseType = releaseType;
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
		UrlWeb = simpleShow.Href;
		Artists =
		[
			new("0", simpleShow.Publisher)
		];
		ReleaseType = ReleaseType.Podcasts;
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
}
