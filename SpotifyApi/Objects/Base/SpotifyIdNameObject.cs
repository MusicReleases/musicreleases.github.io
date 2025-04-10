﻿namespace JakubKastner.SpotifyApi.Objects.Base;

public class SpotifyIdNameObject : IComparable
{
	public required string Id { get; init; }
	public required string Name { get; init; }

	public int CompareTo(object? obj)
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
