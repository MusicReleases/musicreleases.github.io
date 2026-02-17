using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Artists;

public partial class ArtistSidebarFilter
{
	[Parameter, EditorRequired]
	public bool Loading { get; set; }

	private readonly MenuType _type = MenuType.Artists;
}
