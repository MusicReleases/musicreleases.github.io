using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Playlists;
using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Playlists;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Playlists;

public partial class PlaylistPicker : IDisposable
{
	[Inject]
	private ISpotifyPlaylistFilterService FilterService { get; set; } = default!;

	[Inject]
	private ILoadingService LoadingService { get; set; } = default!;


	[Parameter]
	public PlaylistEnums PlaylistTypeFilter { get; set; } = PlaylistEnums.Editable;

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

		FilterService.SetTypeFilter(PlaylistTypeFilter);
	}

	private void FilterChanged()
	{
		_playlists = FilterService.FilteredPlaylists;
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
