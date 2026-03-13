using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Playlists;

public partial class PlaylistPicker : IDisposable
{
	[Inject]
	private ISpotifyFilterPlaylistService FilterService { get; set; } = default!;

	[Inject]
	private ILoadingService LoadingService { get; set; } = default!;


	[Parameter]
	public PlaylistType PlaylistTypeFilter { get; set; } = PlaylistType.Editable;

	[Parameter]
	public SpotifyRelease? Release { get; set; }

	[Parameter]
	public SpotifyTrack? Track { get; set; }


	private bool Loading => LoadingService.IsLoading(BackgroundTaskType.PlaylistsGet) || LoadingService.IsLoading(BackgroundTaskType.PlaylistTracksGet);

	private List<SpotifyPlaylist>? Playlists => FilterService.FilteredPlaylists?.ToList();


	protected override void OnParametersSet()
	{
		if (Release is null && Track is null)
		{
			throw new InvalidOperationException($"You must provide either {nameof(Release)} or {nameof(Track)}.");
		}

		if (Release is not null && Track is not null)
		{
			throw new InvalidOperationException($"You must provide only {nameof(Release)} or {nameof(Track)}, not both.");
		}

		FilterService.SetTypeFilter(PlaylistTypeFilter);
	}

	protected override void OnInitialized()
	{
		FilterService.OnFilterChanged += StateChanged;
		LoadingService.LoadingStateChanged += StateChanged;
	}

	public void Dispose()
	{
		FilterService.OnFilterChanged -= StateChanged;
		LoadingService.LoadingStateChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void SearchTextChanged(string searchText)
	{
		FilterService.SetSearchText(searchText);
	}
}
