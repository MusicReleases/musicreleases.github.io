using JakubKastner.SpotifyApi.Objects.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyArtist : SpotifyIdNameUrlObject, IComparable
{
	public required bool New { get; init; }

	public SpotifyArtist()
	{
		// ctor for json
	}

	[SetsRequiredMembers]
	public SpotifyArtist(string id, string name, string urlApp, string urlWeb, bool isNew) : base(id, name, urlApp, urlWeb)
	{
		New = isNew;
	}
}