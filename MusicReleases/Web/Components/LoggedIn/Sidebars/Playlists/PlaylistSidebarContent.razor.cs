using JakubKastner.SpotifyApi.Playlists;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Playlists;

public partial class PlaylistSidebarContent
{
	[Parameter]
	public IReadOnlySet<SpotifyPlaylist>? Playlists { get; set; }
}