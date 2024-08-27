using JakubKastner.SpotifyApi.Base;
using SpotifyAPI.Web;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyRelease : SpotifyIdNameObject, IComparable
{
	public DateTime ReleaseDate { get; init; }
	public int TotalTracks { get; init; }

	public string UrlApp { get; init; }
	public string UrlWeb { get; init; }
	public string UrlImage { get; init; }

	public string ArtistString => string.Join(", ", Artists.Select(x => x.Name));
	public string ReleaseDateString => ReleaseDate.ToString("dd.MM.yyyy");

	public List<Image> Images { get; init; }

	public HashSet<SpotifyArtist> Artists { get; init; }

	public SortedSet<SpotifyTrack>? Tracks { get; init; }

	public ReleaseType ReleaseType { get; init; }

	// TODO artists - GetArtists
	// TODO images (0), default
	public SpotifyRelease() : base("json", "init")
	{
		// TODO ctor for json
	}

	public SpotifyRelease(SimpleAlbum simpleAlbum, ReleaseType releaseType) : base(simpleAlbum.Id, simpleAlbum.Name)
	{
		ReleaseDate = simpleAlbum.ReleaseDate.ToDateTime();
		TotalTracks = simpleAlbum.TotalTracks;
		Images = simpleAlbum.Images;
		if (simpleAlbum.Images.Count > 0)
		{
			UrlImage = simpleAlbum.Images.First().Url;
		}
		else
		{
			UrlImage = "";
		}
		UrlApp = simpleAlbum.Uri;
		UrlWeb = simpleAlbum.Href;
		Artists = simpleAlbum.Artists.Select(simpleArtist => new SpotifyArtist(simpleArtist)).ToHashSet();
		ReleaseType = releaseType;
	}

	public SpotifyRelease(FullAlbum fullAlbum, ReleaseType releaseType) : base(fullAlbum.Id, fullAlbum.Name)
	{
		ReleaseDate = fullAlbum.ReleaseDate.ToDateTime();
		TotalTracks = fullAlbum.TotalTracks;
		Images = fullAlbum.Images;
		if (fullAlbum.Images.Count > 0)
		{
			UrlImage = fullAlbum.Images.First().Url;
		}
		else
		{
			UrlImage = "";
		}
		UrlApp = fullAlbum.Uri;
		UrlWeb = fullAlbum.Href;
		Artists = fullAlbum.Artists.Select(simpleArtist => new SpotifyArtist(simpleArtist)).ToHashSet();
		ReleaseType = releaseType;
	}

	public SpotifyRelease(SimpleShow simpleShow) : base(simpleShow.Id, simpleShow.Name)
	{
		// TODO podcast release date
		//ReleaseDate = null; 
		TotalTracks = 1;
		Images = simpleShow.Images;
		if (simpleShow.Images.Count > 0)
		{
			UrlImage = simpleShow.Images.First().Url;
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
