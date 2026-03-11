using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseToolbar : IDisposable
{
	[Inject]
	public ILoadingService LoadingService { get; set; } = default!;

	[Inject]
	public ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;


	private bool Loading => LoadingService.IsLoading(BackgroundTaskType.Releases);


	private const string _buttonClass = "toolbar-releases";

	private string? SearchText => SpotifyReleaseFilterService.Filter.SearchText;


	protected override void OnInitialized()
	{
		LoadingService.LoadingStateChanged += StateChanged;
		SpotifyReleaseFilterService.OnFilterChanged += StateChanged;
	}

	public void Dispose()
	{
		LoadingService.LoadingStateChanged -= StateChanged;
		SpotifyReleaseFilterService.OnFilterChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task Search(string? newSearchText)
	{
		SpotifyReleaseFilterService.SetSearch(newSearchText);
	}

	private void AddToPlaylist()
	{

	}

	private void Play()
	{

	}
}
