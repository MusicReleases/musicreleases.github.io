using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseSection : IDisposable
{
	[Inject]
	public ILoaderService LoaderService { get; set; } = default!;

	[Inject]
	private IMobileService MobileService { get; set; } = default!;

	[Parameter]
	public RenderFragment? ChildContent { get; set; }


	private bool Loading => LoaderService.IsLoading(LoadingType.Releases);

	private string ClassHide => MobileService.DisplayMobile == DisplayMobile.Releases ? "show" : string.Empty;


	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += StateChanged;
		MobileService.OnDisplayChanged += StateChanged;
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= StateChanged;
		MobileService.OnDisplayChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
