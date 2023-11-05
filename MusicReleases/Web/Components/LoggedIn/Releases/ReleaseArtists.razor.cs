using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseArtists
{
	[Parameter, EditorRequired]
	public HashSet<SpotifyArtist> ArtistsObj { get; set; }

	private int _index = 0;
}
