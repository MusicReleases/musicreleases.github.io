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
	private ILoadingService LoadingService { get; set; } = default!;


	private bool Loading => LoadingService.IsLoading(BackgroundTaskType.Releases) || LoadingService.IsLoading(BackgroundTaskType.Artists);

	private ISet<SpotifyRelease>? FilteredReleases => SpotifyReleaseFilterService.FilteredReleases;


	protected override void OnInitialized()
	{
		LoadingService.LoadingStateChanged += StateChanged;
		SpotifyReleaseFilterService.OnDataFiltered += StateChanged;
	}

	public void Dispose()
	{
		LoadingService.LoadingStateChanged -= StateChanged;
		SpotifyReleaseFilterService.OnDataFiltered -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
