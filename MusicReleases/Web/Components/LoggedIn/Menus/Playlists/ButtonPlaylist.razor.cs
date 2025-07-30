using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class ButtonPlaylist
{
	[Parameter, EditorRequired]
	public required string PlaylistId { get; set; }

	[Parameter, EditorRequired]
	public required string PlaylistName { get; set; }

	[Parameter]
	public SpotifyRelease? Release { get; set; }

	private async Task AddToPlaylist(bool positionTop)
	{
		if (Release is null)
		{
			return;
		}


	}
}