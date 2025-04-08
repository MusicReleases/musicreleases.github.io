using JakubKastner.MusicReleases.Base;
using Microsoft.AspNetCore.Components;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseIcon
{
	[Parameter]
	public required ReleaseType ReleaseType { get; set; }
	public string IconClass => "fas fa-" + Icons.GetIconForRelease(ReleaseType);
	public string Title => ReleaseType.ToString() + " release";
}
