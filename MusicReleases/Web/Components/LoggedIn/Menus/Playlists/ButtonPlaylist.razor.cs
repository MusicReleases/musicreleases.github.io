using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class ButtonPlaylist
{
	[Parameter, EditorRequired]
	public required string Name { get; set; }
	[Parameter]
	public string? Class { get; set; }

	[Parameter]
	public bool IsLoading { get; set; } = false;
}