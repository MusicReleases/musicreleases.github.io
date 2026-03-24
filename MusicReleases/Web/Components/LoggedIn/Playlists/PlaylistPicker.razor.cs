using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Playlists;
using JakubKastner.SpotifyApi.Playlists;
using JakubKastner.SpotifyApi.Releases;
using JakubKastner.SpotifyApi.Tracks;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Playlists;

public partial class PlaylistPicker : IDisposable
{
	[Inject]
	private ISpotifyPlaylistFilterService FilterService { get; set; } = default!;

	[Inject]
	private ILoadingService LoadingService { get; set; } = default!;


	[Parameter]
	public SpotifyPlaylistType PlaylistTypeFilter { get; set; } = SpotifyPlaylistType.Editable;

	[Parameter]
	public SpotifyRelease? Release { get; set; }

	[Parameter]
	public SpotifyTrack? Track { get; set; }


	private bool IsLoading => LoadingService.IsLoading(BackgroundTaskType.PlaylistsGet) || LoadingService.IsLoading(BackgroundTaskType.PlaylistTracksGet);

	private IReadOnlySet<SpotifyPlaylist>? _playlists;

	protected override void OnInitialized()
	{
		FilterService.OnChanged += FilterChanged;
		LoadingService.LoadingStateChanged += StateChanged;
	}

	public void Dispose()
	{
		FilterService.OnChanged -= FilterChanged;
		LoadingService.LoadingStateChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

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

		var filterChanged = FilterService.SetTypeFilter(PlaylistTypeFilter);

		if (!filterChanged)
		{
			RecalculateFilter();
		}
	}

	private void RecalculateFilter()
	{
		_playlists = FilterService.FilteredPlaylists;
	}

	private void FilterChanged()
	{
		RecalculateFilter();
		StateChanged();
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void Search(string? searchText)
	{
		FilterService.SetSearchText(searchText);
	}
}
