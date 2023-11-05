using JakubKastner.MusicReleases.Base;
using Microsoft.AspNetCore.Components;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class ButtonRelease
{
    [Parameter, EditorRequired]
    public ReleaseType ReleaseType { get; set; }

    private string? _releaseType;
    private string? _icon;

    protected override void OnInitialized()
    {
        _releaseType = ReleaseType.ToString();
        _icon = Icons.GetIconForRelease(ReleaseType);
    }

    private void DisplayReleases()
    {
        _navManager.NavigateTo("releases/" + _releaseType?.ToLower());
    }
}
