using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class Release
{
	[Parameter]
	public required SpotifyRelease SpotifyRelease { get; set; }
}
