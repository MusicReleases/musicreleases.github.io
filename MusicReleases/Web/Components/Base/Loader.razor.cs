using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class Loader : IDisposable
{
	[Inject]
	private ILoadingService LoadingService { get; set; } = default!;


	protected override void OnInitialized()
	{
		LoadingService.LoadingStateChanged += StateChanged;
	}

	public void Dispose()
	{
		LoadingService.LoadingStateChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
