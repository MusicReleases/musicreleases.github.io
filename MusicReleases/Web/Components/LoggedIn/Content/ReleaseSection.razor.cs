using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseSection : IDisposable
{
	[Inject]
	public ILoaderService LoaderService { get; set; } = default!;


	[Parameter]
	public RenderFragment? ChildContent { get; set; }


	private bool Loading => LoaderService.IsLoading(LoadingType.Releases);


	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += StateChanged;
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
