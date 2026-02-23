using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseArtists
{
	[Parameter, EditorRequired]
	public required HashSet<SpotifyArtist> Artists { get; set; }

	[Parameter]
	public string? ButtonClass { get; set; }

	[Parameter]
	public string? SpanClass { get; set; }
}
