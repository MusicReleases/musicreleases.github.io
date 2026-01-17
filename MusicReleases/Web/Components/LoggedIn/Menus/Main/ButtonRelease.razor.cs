using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Main;

public partial class ButtonRelease
{
	[Parameter, EditorRequired]
	public ReleaseType ReleaseType { get; set; }

	private string ReleaseTypeText => ReleaseType.ToString();
	private string ButtonTitle => $"View released {ReleaseTypeText}";
	private string ButtonClass => $"main-menu rounded-xl trasparent{(ReleaseFilter ? " active" : string.Empty)}";
	private LucideIcon Icon => EnumIconsExtensions.GetIconForRelease(ReleaseType);

	private bool ReleaseFilter => SpotifyFilterService.Filter?.ReleaseType == ReleaseType;

	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
		GC.SuppressFinalize(this);
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
