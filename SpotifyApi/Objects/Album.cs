using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class Album : IComparable
{
	public string Id { get; private set; }
	public string Name { get; private set; }
	public string ReleaseDate { get; private set; }
	public string AlbumType { get; private set; }
	public int TotalTracks { get; private set; }
	public string Uri { get; private set; }

	public string ImageUrl { get; private set; }

	public List<Image> Images { get; private set; }

	public HashSet<Artist> Artists { get; private set; }

	public SortedSet<Track> Tracks { get; private set; } = new();

	// TODO artists - GetArtists
	// TODO images (0), default
	public Album(SimpleAlbum simpleAlbum)
	{
		Id = simpleAlbum.Id;
		Name = simpleAlbum.Name;
		ReleaseDate = simpleAlbum.ReleaseDate;
		AlbumType = simpleAlbum.AlbumType;
		TotalTracks = simpleAlbum.TotalTracks;
		Images = simpleAlbum.Images;
		if (simpleAlbum.Images.Count > 0)
		{
			ImageUrl = simpleAlbum.Images.First().Url;
		}
		else
		{
			ImageUrl = "";
		}
		Uri = simpleAlbum.Uri;
		//Artists = new();
		Artists = simpleAlbum.Artists.Select(simpleArtist => new Artist(simpleArtist)).ToHashSet();
		//Artists = Controller.GetArtists(simpleAlbum.Artists);
	}
	public Album(FullAlbum fullAlbum)
	{
		Id = fullAlbum.Id;
		Name = fullAlbum.Name;
		ReleaseDate = fullAlbum.ReleaseDate;
		AlbumType = fullAlbum.AlbumType;
		TotalTracks = fullAlbum.TotalTracks;
		Images = fullAlbum.Images;
		if (fullAlbum.Images.Count > 0)
		{
			ImageUrl = fullAlbum.Images.First().Url;
		}
		else
		{
			ImageUrl = "";
		}
		Uri = fullAlbum.Uri;
		//Artists = new();
		Artists = fullAlbum.Artists.Select(simpleArtist => new Artist(simpleArtist)).ToHashSet();
		//Artists = Controller.GetArtists(fullAlbum.Artists);
	}
	public Album(SimpleShow simpleShow)
	{
		Id = simpleShow.Id;
		Name = simpleShow.Name;
		ReleaseDate = "0";
		AlbumType = "Podcast";
		TotalTracks = 1;
		Images = simpleShow.Images;
		if (simpleShow.Images.Count > 0)
		{
			ImageUrl = simpleShow.Images.First().Url;
		}
		else
		{
			ImageUrl = "";
		}
		Uri = simpleShow.Uri;
		Artists = new()
		{
			new(id: "0", name: simpleShow.Publisher)
		};
	}

	public int CompareTo(object? obj)
	{
		if (obj == null)
		{
			return -1;
		}

		var other = (Album)obj;
		var releaseDateComparison = -ReleaseDate.CompareTo(other.ReleaseDate);

		return (releaseDateComparison != 0) ? releaseDateComparison : Name.CompareTo(other.Name);
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return this == null;
		}

		var other = (Album)obj;
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
