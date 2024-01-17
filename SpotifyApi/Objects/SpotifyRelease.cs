using SpotifyAPI.Web;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyRelease : IComparable
{
	public string Id { get; private set; }
	public string Name { get; private set; }
	public string ReleaseDate { get; private set; }
	public int TotalTracks { get; private set; }

	public string UrlApp { get; private set; }
	public string UrlWeb { get; private set; }
	public string UrlImage { get; private set; }

	public List<Image> Images { get; private set; }

	public HashSet<SpotifyArtist> Artists { get; private set; }

	public SortedSet<SpotifyTrack>? Tracks { get; private set; }

	public ReleaseType ReleaseType { get; private set; }

	// TODO artists - GetArtists
	// TODO images (0), default

	public SpotifyRelease(SimpleAlbum simpleAlbum, ReleaseType releaseType)
	{
		Id = simpleAlbum.Id;
		Name = simpleAlbum.Name;
		ReleaseDate = simpleAlbum.ReleaseDate;
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

	public SpotifyRelease(FullAlbum fullAlbum, ReleaseType releaseType)
	{
		Id = fullAlbum.Id;
		Name = fullAlbum.Name;
		ReleaseDate = fullAlbum.ReleaseDate;
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

	public SpotifyRelease(SimpleShow simpleShow)
	{
		Id = simpleShow.Id;
		Name = simpleShow.Name;
		ReleaseDate = "0";
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
			new(id: "0", name: simpleShow.Publisher)
		];
		ReleaseType = ReleaseType.Podcasts;
	}

	public int CompareTo(object? obj)
	{
		if (obj == null)
		{
			return -1;
		}

		var other = (SpotifyRelease)obj;
		var releaseDateComparison = -ReleaseDate.CompareTo(other.ReleaseDate);

		return (releaseDateComparison != 0) ? releaseDateComparison : Name.CompareTo(other.Name);
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return this == null;
		}

		var other = (SpotifyRelease)obj;
		var idEquals = string.Equals(Id, other.Id);
		if (idEquals)
		{
			return true;
		}

		return false;
		// TODO equals name and artists (and mark explicit)
		/*var nameEquals = string.Equals(Name, other.Name) && other.Artists.Contains(Artists.First());
		return nameEquals;*/
	}

	public override int GetHashCode()
	{
		return Id.GetHashCode();
	}
}
