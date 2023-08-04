using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Artists;

public partial class ButtonArtist
{
    [Parameter, EditorRequired]
    public string? ArtistId { get; set; }

    [Parameter, EditorRequired]
    public string? ArtistName { get; set; }
}