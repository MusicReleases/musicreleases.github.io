namespace JakubKastner.SpotifyApi.Objects.Base;

public class SpotifyIdNameUrlObject : IComparable
{
	public required string Id { get; init; }
	public required string Name { get; init; }
	public required string UrlApp { get; init; }
	public required string UrlWeb { get; init; }

	public int CompareTo(object? obj)
	{
		if (obj == null)
		{
			return -1;
		}

		var other = (SpotifyIdNameUrlObject)obj;
		var nameComparison = Name.CompareTo(other.Name);

		return nameComparison != 0 ? nameComparison : Id.CompareTo(other.Id);
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return this == null;
		}

		var other = (SpotifyIdNameUrlObject)obj;
		return string.Equals(Id, other.Id);
	}

	public override int GetHashCode()
	{
		return Id?.GetHashCode() ?? 0;
	}
}
