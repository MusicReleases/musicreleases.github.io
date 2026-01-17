using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseIcon
{
	[Parameter]
	public required ReleaseType ReleaseType { get; set; }
	public string IconClass => "fas fa-" + EnumIconsExtensions.GetIconForReleaseOld(ReleaseType);
	public string Title => ReleaseType.ToString() + " release";
}
