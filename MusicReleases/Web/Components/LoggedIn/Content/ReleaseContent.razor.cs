using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseContent : IDisposable
{
	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;

	[Inject]
	private ILoaderService LoaderService { get; set; } = default!;


	private bool Loading => LoaderService.IsLoading(LoadingType.Releases) || LoaderService.IsLoading(LoadingType.Artists);

	private ISet<SpotifyRelease>? FilteredReleases => SpotifyReleaseFilterService.FilteredReleases;


	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += StateChanged;
		SpotifyReleaseFilterService.OnFilterOrDataChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyReleaseFilterService.Dispose();
		LoaderService.LoadingStateChanged -= StateChanged;
		SpotifyReleaseFilterService.OnFilterOrDataChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
