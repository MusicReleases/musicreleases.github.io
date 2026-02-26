using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseContent : IDisposable
{
	[Inject]
	private ISpotifyWorkflowService SpotifyWorkflowService { get; set; } = default!;

	[Inject]
	private ISpotifyFilterService SpotifyFilterService { get; set; } = default!;

	[Inject]
	private ILoaderService LoaderService { get; set; } = default!;


	private bool Loading => LoaderService.IsLoading(LoadingType.Releases) || LoaderService.IsLoading(LoadingType.Artists);

	private ISet<SpotifyRelease>? FilteredReleases => SpotifyFilterService.FilteredReleases;


	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += StateChanged;
		SpotifyFilterService.OnFilterOrDataChanged += StateChanged;
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= StateChanged;
		SpotifyFilterService.OnFilterOrDataChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
