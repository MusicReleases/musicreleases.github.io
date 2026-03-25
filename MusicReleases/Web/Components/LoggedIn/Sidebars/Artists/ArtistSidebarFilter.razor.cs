using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Artists;
using JakubKastner.MusicReleases.Spotify.Tasks;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars.Artists;

public partial class ArtistSidebarFilter : IDisposable
{
	[Inject]
	public ILoadingService LoadingService { get; set; } = default!;

	[Inject]
	private ISpotifyArtistFilterService FilterService { get; set; } = default!;


	private bool IsLoading => LoadingService.IsLoading(BackgroundTaskType.ArtistsGet);


	private const string _buttonClass = "sidebar-filter";

	private string? SearchText => FilterService.SearchText;


	protected override void OnInitialized()
	{
		LoadingService.LoadingStateChanged += StateChanged;
		FilterService.OnSearchTextChanged += StateChanged;
	}

	public void Dispose()
	{
		LoadingService.LoadingStateChanged -= StateChanged;
		FilterService.OnSearchTextChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void Search(string? newSearchText)
	{
		FilterService.SetSearch(newSearchText);
	}
}
