using JakubKastner.SpotifyApi.Objects.Base;
using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyArtist : SpotifyIdNameUrlObject, IComparable
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
		UrlApp = simpleArtist.Uri;
		UrlWeb = simpleArtist.Href;
		New = true;
	}

	[SetsRequiredMembers]
	public SpotifyArtist(FullArtist fullArtist)
	{
		Id = fullArtist.Id;
		Name = fullArtist.Name;
		UrlApp = fullArtist.Uri;
		UrlWeb = fullArtist.Href;
		New = true;
	}

	[SetsRequiredMembers]
	public SpotifyArtist(string id, string name, string urlApp, string urlWeb)
	{
		Id = id;
		Name = name;
		UrlApp = urlApp;
		UrlWeb = urlWeb;
		New = true;
	}
}