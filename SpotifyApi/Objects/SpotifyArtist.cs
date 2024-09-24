using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyArtist : SpotifyIdNameObject
{
	public SortedSet<SpotifyRelease>? Releases { get; set; }
	public bool New { get; init; } = false;

	public SpotifyArtist()
	{
		// TODO ctor for json
	}

	[SetsRequiredMembers]
	public SpotifyArtist(SimpleArtist simpleArtist)
	{
		Id = simpleArtist.Id;
		Name = simpleArtist.Name;
		New = true;
	}

	[SetsRequiredMembers]
	public SpotifyArtist(FullArtist fullArtist)
	{
		Id = fullArtist.Id;
		Name = fullArtist.Name;
		New = true;
	}

	[SetsRequiredMembers]
	public SpotifyArtist(string id, string name)
	{
		Id = id;
		Name = name;
		New = true;
	}
}