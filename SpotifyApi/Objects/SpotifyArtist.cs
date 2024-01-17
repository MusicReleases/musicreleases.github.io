using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyArtist : IComparable
{
	public string? Id { get; set; }
	public string? Name { get; set; }
	public SortedSet<SpotifyRelease>? Releases { get; set; }

	public SpotifyArtist() { }

	public SpotifyArtist(SimpleArtist simpleArtist)
	{
		Id = simpleArtist.Id;
		Name = simpleArtist.Name;
	}
	public SpotifyArtist(FullArtist fullArtist)
	{
		Id = fullArtist.Id;
		Name = fullArtist.Name;
	}
	public SpotifyArtist(string id, string name)
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

		var other = (SpotifyArtist)obj;

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

		var other = (SpotifyArtist)obj;
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
