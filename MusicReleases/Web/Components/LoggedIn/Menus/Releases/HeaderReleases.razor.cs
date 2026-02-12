using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class HeaderReleases
{
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? Attributes { get; set; }
	private bool Loading => LoaderService.IsLoading(LoadingType.Artists) || LoaderService.IsLoading(LoadingType.Releases);

	private readonly MenuType _type = MenuType.Releases;

	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += LoadingStateChanged;
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= LoadingStateChanged;
		GC.SuppressFinalize(this);
	}

	private void LoadingStateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void AddToPlaylist()
	{
	}
	private void Play()
	{
	}
}
