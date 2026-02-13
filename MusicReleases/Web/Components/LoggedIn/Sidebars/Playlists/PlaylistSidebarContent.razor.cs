using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Playlists;

public partial class PlaylistSidebarContent
{
	[Parameter]
	public ICollection<SpotifyPlaylist>? Playlists { get; set; }
}