using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Popups;

public partial class TaskPopup : IDisposable
{
	[Inject]
	private ISpotifyTaskManagerService SpotifyTaskManagerService { get; set; } = default!;

	[Inject]
	private ISpotifyTaskFilterService SpotifyTaskFilterService { get; set; } = default!;


	private ICollection<SpotifyBackgroundTask> DisplayedTasks => SpotifyTaskManagerService.FilteredTasks;

	private string ClearFilterButtonText => SpotifyTaskFilterService.IsFilterActive ? "Clear all task filters" : "No filters enabled";

	private LucideIcon ClearFilterIcon => SpotifyTaskFilterService.IsFilterActive ? LucideIcon.FunnelX : LucideIcon.Funnel;

	private string ZeroTasksText => SpotifyTaskFilterService.IsFilterActive ? "No tasks match the current filters." : "No tasks are in history.";


	protected override void OnInitialized()
	{
		SpotifyTaskManagerService.OnChange += StateChanged;
		SpotifyTaskFilterService.OnFilterChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyTaskManagerService.OnChange -= StateChanged;
		SpotifyTaskFilterService.OnFilterChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void ClearFilter()
	{
		SpotifyTaskFilterService.ClearFilter();
	}
}
