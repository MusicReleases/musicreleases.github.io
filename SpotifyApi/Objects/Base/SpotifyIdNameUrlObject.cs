using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects.Base;

[method: SetsRequiredMembers]
public abstract class SpotifyIdNameUrlObject(string id, string name, string urlApp, string urlWeb) : SpotifyIdNameObject(id, name), IComparable
{
	public required string UrlApp { get; init; } = urlApp;

	public required string UrlWeb { get; init; } = urlWeb;
}
