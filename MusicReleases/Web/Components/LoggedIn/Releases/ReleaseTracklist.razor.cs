using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseTracklist
{
	[Parameter, EditorRequired]
	public required SpotifyRelease Release { get; set; }
}
