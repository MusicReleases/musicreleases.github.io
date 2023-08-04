using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class ButtonPlaylist
{
	[Parameter, EditorRequired]
	public string PlaylistId { get; set; } = string.Empty;

	[Parameter, EditorRequired]
	public string PlaylistName { get; set; } = string.Empty;
}