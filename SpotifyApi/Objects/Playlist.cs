using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class Playlist : IComparable
{
	public string Id { get; private set; }
	public string Name { get; private set; }
	public bool CurrentUserOwned { get; private set; }
	public bool Collaborative { get; private set; }

	public int? TotalTracks { get; private set; }

	public HashSet<Track> Tracks { get; set; } = new();

	// TODO playlist owner - currentuserowned
	public Playlist(SimplePlaylist simplePlaylist, bool currentUserOwned = false)
	{
		Id = simplePlaylist.Id;
		Name = simplePlaylist.Name;
		TotalTracks = simplePlaylist.Tracks.Total;
		Collaborative = simplePlaylist.Collaborative;
		CurrentUserOwned = currentUserOwned;
	}
	public Playlist(FullPlaylist fullPlaylist, bool currentUserOwned = false)
	{
		// TODO null
		Id = fullPlaylist.Id ?? "";
		Name = fullPlaylist.Name ?? "";
		TotalTracks = fullPlaylist.Tracks?.Total;
		Collaborative = fullPlaylist.Collaborative ?? false;
		CurrentUserOwned = currentUserOwned;
	}

	public int CompareTo(object? obj)
	{
		if (obj == null)
		{
			return -1;
		}

		var other = (Playlist)obj;
		var nameComparison = Name.CompareTo(other.Name);

		return (nameComparison != 0) ? nameComparison : Id.CompareTo(other.Id);
	}

	public override bool Equals(object? obj)
	{
		if (obj == null)
		{
			return this == null;
		}

		var other = (Playlist)obj;
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
