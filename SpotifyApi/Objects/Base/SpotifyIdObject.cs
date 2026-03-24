using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects.Base;

[method: SetsRequiredMembers]
public abstract class SpotifyIdObject(string id) : IComparable
{
	public required string Id { get; init; } = id;

	public int CompareTo(object? obj)
	{
		if (obj == null)
		{
			return -1;
		}

		var other = (SpotifyIdObject)obj;

		return Id.CompareTo(other.Id);
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return this == null;
		}

		var other = (SpotifyIdObject)obj;
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
