using JakubKastner.MusicReleases.Base;
using Microsoft.AspNetCore.Components;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class ButtonRelease
{
	[Parameter, EditorRequired]
	public ReleaseType ReleaseType { get; set; }

	private string ReleaseTypeString => ReleaseType.ToString();
	private string ButtonClass => ReleaseTypeString.ToLower() + (ReleaseFilter ? " active" : string.Empty);
	private string IconClass => Icons.GetIconForRelease(ReleaseType);

	private bool ReleaseFilter => SpotifyFilterState.Value.Filter.ReleaseType == ReleaseType;

	private void DisplayReleases()
	{
		var url = SpotifyFilterService.GetFilterUrl(ReleaseType);
		NavManager.NavigateTo(url);
	}
}
