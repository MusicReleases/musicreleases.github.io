using JakubKastner.SpotifyApi.Objects.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

[method: SetsRequiredMembers]
public class SpotifyArtist(string id, string name, string urlApp, string urlWeb, bool isNew) : SpotifyIdNameUrlObject(id, name, urlApp, urlWeb), IComparable
{
	public required bool New { get; init; } = isNew;
}