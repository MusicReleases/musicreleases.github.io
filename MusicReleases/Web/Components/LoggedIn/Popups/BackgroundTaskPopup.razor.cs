using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Popups;

public partial class BackgroundTaskPopup : IDisposable
{
	[Inject]
	private IBackgroundTaskManagerService SpotifyTaskManagerService { get; set; } = default!;

	[Inject]
	private IBackgroundTaskFilterService SpotifyTaskFilterService { get; set; } = default!;


	private ICollection<BackgroundTask> DisplayedTasks => SpotifyTaskManagerService.FilteredTasks;

	private string ClearFilterButtonTitle => SpotifyTaskFilterService.IsFilterActive ? "Clear all task filters" : "No task filters applied";

	private LucideIcon ClearFilterIcon => SpotifyTaskFilterService.IsFilterActive ? LucideIcon.FunnelX : LucideIcon.Funnel;

	private string ZeroTasksText => SpotifyTaskFilterService.IsFilterActive ? "No tasks match the current filters." : (SpotifyTaskFilterService.IsSearching ? "No tasks match the current searching." : "No tasks are in history.");


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

	private void Search(string searchText)
	{
		SpotifyTaskFilterService.SetSearch(searchText);
	}

	private void DeleteFinished()
	{
		SpotifyTaskManagerService.RemoveAllFinishedTasks();
	}
}
