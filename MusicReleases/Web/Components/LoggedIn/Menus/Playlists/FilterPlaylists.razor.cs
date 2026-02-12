using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class FilterPlaylists
{
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? Attributes { get; set; }

	[Parameter]
	public string SearchTerm { get; set; } = string.Empty;
	[Parameter]
	public EventCallback<string> SearchTermChanged { get; set; }

	private bool Loading => LoaderService.IsLoading(LoadingType.Playlists) || LoaderService.IsLoading(LoadingType.PlaylistTracks);

	private readonly MenuType _type = MenuType.Playlists;

	private async Task OnSearch(string newSearchTerm)
	{
		SearchTerm = newSearchTerm;

		if (SearchTermChanged.HasDelegate)
		{
			await SearchTermChanged.InvokeAsync(newSearchTerm);
		}
	}

	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += LoadingStateChanged;
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= LoadingStateChanged;
		GC.SuppressFinalize(this);
	}

	private void LoadingStateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task CreatePlaylist()
	{
		if (SearchTerm.IsNullOrEmpty())
		{
			return;
		}

		await SpotifyPlaylistService.CreatePlaylist(SearchTerm);
		await ClearSearch();
	}

	private async Task ClearSearch()
	{
		SearchTerm = string.Empty;
		await OnSearch(SearchTerm);
	}
}
