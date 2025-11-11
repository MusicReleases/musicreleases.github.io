using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseTrack
{
	[Parameter, EditorRequired]
	public required SpotifyTrack Track { get; set; }
}
