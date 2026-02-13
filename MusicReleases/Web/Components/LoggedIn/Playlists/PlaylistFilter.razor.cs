using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Playlists;

public partial class PlaylistFilter
{
	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? Attributes { get; set; }

	[Parameter]
	public string SearchText { get; set; } = string.Empty;
	[Parameter]
	public EventCallback<string> SearchTextChanged { get; set; }


	private readonly MenuType _type = MenuType.Playlists;

	private async Task OnSearch(string newSearchText)
	{
		SearchText = newSearchText;

		if (SearchTextChanged.HasDelegate)
		{
			await SearchTextChanged.InvokeAsync(newSearchText);
		}
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
		SearchText = string.Empty;
		await OnSearch(SearchText);
	}
}
