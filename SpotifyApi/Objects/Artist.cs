using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class Artist : IComparable
{
	public string? Id { get; set; }
	public string? Name { get; set; }

	public Artist() { }

	public Artist(SimpleArtist simpleArtist)
	{
		Id = simpleArtist.Id;
		Name = simpleArtist.Name;
	}
	public Artist(FullArtist fullArtist)
	{
		Id = fullArtist.Id;
		Name = fullArtist.Name;
	}
	public Artist(string id, string name)
	{
		Id = id;
		Name = name;
	}

	public int CompareTo(object? obj)
	{
		if (obj == null)
		{
			return -1;
		}

		var other = (Artist)obj;

		var nameComparison = Name?.CompareTo(other.Name);
		if (nameComparison.HasValue && nameComparison != 0)
		{
			return nameComparison.Value;
		}

		var idComparison = Id?.CompareTo(other.Id);
		if (idComparison.HasValue)
		{
			return idComparison.Value;
		}

		return -1;
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return this == null;
		}

		var other = (Artist)obj;
		return string.Equals(Id, other.Id);
	}

	public override int GetHashCode()
	{
		if (Id == null)
		{
			return new();
		}
		return Id.GetHashCode();
	}
}
