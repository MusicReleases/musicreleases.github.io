using JakubKastner.SpotifyApi.Base.Objects;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Artists;

[method: SetsRequiredMembers]
public class SpotifyArtist(string id, string name, string urlApp, string urlWeb, bool isNew) : SpotifyIdNameUrlObject(id, name, urlApp, urlWeb), IComparable
{
	public required bool New { get; set; } = isNew;
}