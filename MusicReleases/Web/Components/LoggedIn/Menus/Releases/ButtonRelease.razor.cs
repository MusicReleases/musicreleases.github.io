using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class ButtonRelease
{
	[Parameter, EditorRequired]
	public ReleaseType ReleaseType { get; set; }

	private string ReleaseTypeString => ReleaseType.ToString();
	private string ButtonClass => ReleaseTypeString.ToLower() + (ReleaseFilter ? " active" : string.Empty);
	private string IconClass => EnumIconsExtensions.GetIconForRelease(ReleaseType);

	private bool ReleaseFilter => SpotifyFilterService.Filter!.ReleaseType == ReleaseType;

	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;
		base.OnInitialized();
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
	}

	private void OnFilterOrDataChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task DisplayReleases()
	{
		var url = await SpotifyFilterUrlService.GetFilterUrl(ReleaseType);
		NavManager.NavigateTo(url);
	}
}
