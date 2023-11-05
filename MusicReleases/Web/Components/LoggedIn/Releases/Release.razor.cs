using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class Release
{
    [Parameter, EditorRequired]
    public SpotifyAlbum ReleaseObj { get; set; }
}
