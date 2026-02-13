using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Playlists;

public partial class PlaylistFilter
{
	[Inject]
	private ISpotifyPlaylistService SpotifyPlaylistService { get; set; } = default!;


	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? Attributes { get; set; }

	[Parameter]
	public string? SearchText { get; set; }

	[Parameter]
	public EventCallback<string> SearchTextChanged { get; set; }


	private readonly MenuType _type = MenuType.Playlists;


	private async Task OnSearch(string? newSearchText)
	{
		await UpdateSearchText(newSearchText);
	}

	private async Task CreatePlaylist()
	{
		if (SearchText.IsNullOrEmpty())
		{
			return;
		}

		await SpotifyPlaylistService.CreatePlaylist(SearchText);
		await ClearSearch();
	}

	private async Task ClearSearch()
	{
		await UpdateSearchText(null);
	}

	private async Task UpdateSearchText(string? newSearchText)
	{
		SearchText = newSearchText;

		if (SearchTextChanged.HasDelegate)
		{
			await SearchTextChanged.InvokeAsync(newSearchText);
		}
	}
}
