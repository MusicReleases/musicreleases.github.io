using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class Loader : IDisposable
{
	[Inject]
	private ILoaderService LoaderService { get; set; } = default!;


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
