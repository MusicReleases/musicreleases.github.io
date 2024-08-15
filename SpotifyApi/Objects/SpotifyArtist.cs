using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyArtist : SpotifyIdObject
{
	public SortedSet<SpotifyRelease>? Releases { get; set; }

	public SpotifyArtist() : this("json", "init")
	{
		// TODO ctor for json
	}

	public SpotifyArtist(SimpleArtist simpleArtist) : base(simpleArtist.Id, simpleArtist.Name)
	{
	}

	public SpotifyArtist(FullArtist fullArtist) : base(fullArtist.Id, fullArtist.Name)
	{
	}

	public SpotifyArtist(string id, string name) : base(id, name)
	{
	}
}