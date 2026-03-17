using JakubKastner.SpotifyApi.Playlists;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Playlists;

public partial class PlaylistList
{
	[Parameter, EditorRequired]
	public required IReadOnlySet<SpotifyPlaylist> Playlists { get; set; }

	[Parameter, EditorRequired]
	public required RenderFragment<SpotifyPlaylist> RowTemplate { get; set; }

	private List<SpotifyPlaylist> PlaylistsVirtualize => [.. Playlists];
}
