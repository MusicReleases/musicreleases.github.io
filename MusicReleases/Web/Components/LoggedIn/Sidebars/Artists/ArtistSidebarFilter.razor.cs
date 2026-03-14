using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Artists;

public partial class ArtistSidebarFilter : IDisposable
{
	[Inject]
	public ILoadingService LoadingService { get; set; } = default!;

	[Inject]
	public ISpotifyArtistFilterService SpotifyArtistFilterService { get; set; } = default!;


	private bool IsLoading => LoadingService.IsLoading(BackgroundTaskType.ArtistsGet);


	private const string _buttonClass = "sidebar-filter";

	private string? SearchText => SpotifyArtistFilterService.SearchText;


	protected override void OnInitialized()
	{
		LoadingService.LoadingStateChanged += StateChanged;
		SpotifyArtistFilterService.OnSearchOrDataChanged += StateChanged;
	}

	public void Dispose()
	{
		LoadingService.LoadingStateChanged -= StateChanged;
		SpotifyArtistFilterService.OnSearchOrDataChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task Search(string? newSearchText)
	{
		SpotifyArtistFilterService.SetSearch(newSearchText);
	}
}
