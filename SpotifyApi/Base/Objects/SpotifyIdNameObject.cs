using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Base.Objects;

[method: SetsRequiredMembers]
public abstract class SpotifyIdNameObject(string id, string name) : SpotifyIdObject(id), IComparable
{
	public required string Name { get; init; } = name;

	public new int CompareTo(object? obj)
	{
		if (obj == null)
		{
			return -1;
		}

		var other = (SpotifyIdNameObject)obj;
		var nameComparison = Name.CompareTo(other.Name);

		return nameComparison != 0 ? nameComparison : Id.CompareTo(other.Id);
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return this == null;
		}

		var other = (SpotifyIdNameObject)obj;
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
