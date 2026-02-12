using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class ListPlaylists
{
	[Parameter, EditorRequired]
	public required RenderFragment<SpotifyPlaylist> RowTemplate { get; set; }

	[Parameter]
	public PlaylistType TypeFilter { get; set; } = PlaylistType.Editable;

	[Parameter]
	public string? SearchText { get; set; }
	[Parameter]
	public EventCallback<string> SearchTextChanged { get; set; }

	private string _localSearch = string.Empty;

	private string CurrentSearchText => SearchTextChanged.HasDelegate ? (SearchText ?? string.Empty) : _localSearch;
	private bool Loading => LoaderService.IsLoading(LoadingType.Playlists);

	private async Task HandleSearchChange(string newValue)
	{
		if (SearchTextChanged.HasDelegate)
		{
			// save to parent
			await SearchTextChanged.InvokeAsync(newValue);
		}
		else
		{
			// save locally
			_localSearch = newValue;
		}
	}

	private List<SpotifyPlaylist>? FilteredPlaylists => FilterService.GetFilteredPlaylists(CurrentSearchText, TypeFilter)?.ToList();

	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += StateHasChanged;
		FilterService.OnFilterChanged += StateHasChanged;
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= StateHasChanged;
		FilterService.OnFilterChanged -= StateHasChanged;
		GC.SuppressFinalize(this);
	}
}
