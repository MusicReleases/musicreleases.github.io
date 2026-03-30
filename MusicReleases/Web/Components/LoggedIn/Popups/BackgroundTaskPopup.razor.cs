using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Spotify.Tasks;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Popups;

public partial class BackgroundTaskPopup : IDisposable
{
	[Inject]
	private IBackgroundTaskState BackgroundTaskState { get; set; } = default!;

	[Inject]
	private IBackgroundTaskManagerService BackgroundTaskManager { get; set; } = default!;

	[Inject]
	private IBackgroundTaskFilterService BackgroundTaskFilterService { get; set; } = default!;

	private ICollection<BackgroundTask> DisplayedTasks => [.. BackgroundTaskState.Tasks];

	private string ClearFilterButtonTitle => BackgroundTaskFilterService.IsFilterActive ? "Clear all task filters" : "No task filters applied";

	private LucideIcon ClearFilterIcon => BackgroundTaskFilterService.IsFilterActive ? LucideIcon.FunnelX : LucideIcon.Funnel;

	private string ZeroTasksText => BackgroundTaskFilterService.IsFilterActive ? "No tasks match the current filters." : (BackgroundTaskFilterService.IsSearching ? "No tasks match the current searching." : "No tasks are in history.");


	protected override void OnInitialized()
	{
		BackgroundTaskState.OnChange += StateChanged;
		//BackgroundTaskFilterService.OnFilterChanged += StateChanged;
	}

	public void Dispose()
	{
		BackgroundTaskState.OnChange -= StateChanged;
		//BackgroundTaskFilterService.OnFilterChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void ClearFilter()
	{
		BackgroundTaskFilterService.ClearFilter();
	}

	private void Search(string searchText)
	{
		BackgroundTaskFilterService.SetSearch(searchText);
	}

	private void DeleteAllCompleted()
	{
		BackgroundTaskManager.RemoveAllCompleted();
	}
}
