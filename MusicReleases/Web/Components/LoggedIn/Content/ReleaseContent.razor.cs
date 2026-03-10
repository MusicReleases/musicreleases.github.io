using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseContent : IDisposable
{
	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;

	[Inject]
	private ILoaderService LoaderService { get; set; } = default!;


	private bool Loading => LoaderService.IsLoading(BackgroundTaskType.Releases) || LoaderService.IsLoading(BackgroundTaskType.Artists);

	private ISet<SpotifyRelease>? FilteredReleases => SpotifyReleaseFilterService.FilteredReleases;


	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += StateChanged;
		SpotifyReleaseFilterService.OnDataFiltered += StateChanged;
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= StateChanged;
		SpotifyReleaseFilterService.OnDataFiltered -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
