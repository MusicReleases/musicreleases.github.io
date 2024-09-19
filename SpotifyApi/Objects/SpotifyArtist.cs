using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyArtist : SpotifyIdNameObject
{
	public SortedSet<SpotifyRelease>? Releases { get; set; }
	public bool New { get; init; } = false;

	public SpotifyArtist() : base("json", "init")
	{
		// TODO ctor for json
	}

	public SpotifyArtist(SimpleArtist simpleArtist) : base(simpleArtist.Id, simpleArtist.Name)
	{
		New = true;
	}

	public SpotifyArtist(FullArtist fullArtist) : base(fullArtist.Id, fullArtist.Name)
	{
		New = true;
	}

	public SpotifyArtist(string id, string name) : base(id, name)
	{
		New = true;
	}
}