using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Playlists;

public partial class PlaylistList
{
	[Parameter, EditorRequired]
	public ICollection<SpotifyPlaylist>? Playlists { get; set; }

	[Parameter, EditorRequired]
	public required RenderFragment<SpotifyPlaylist> RowTemplate { get; set; }
}
