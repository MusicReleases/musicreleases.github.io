using JakubKastner.MusicReleases.Base;
using Microsoft.AspNetCore.Components;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class ButtonRelease
{
	[Parameter, EditorRequired]
	public ReleaseType ReleaseType { get; set; }

	private string ReleaseTypeString => ReleaseType.ToString();
	private string ButtonClass => ReleaseTypeString.ToLower() + (_releaseFilter ? " active" : string.Empty);
	private string? IconClass => Icons.GetIconForRelease(ReleaseType);

	private bool _releaseFilter = false;

	private void DisplayReleases()
	{
		_releaseFilter = !_releaseFilter;

		NavManager.NavigateTo("releases/" + ReleaseTypeString.ToLower());
	}
}
