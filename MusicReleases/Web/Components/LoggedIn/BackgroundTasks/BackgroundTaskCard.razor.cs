using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.BackgroundTasks;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.BackgroundTasks;

public partial class BackgroundTaskCard : IDisposable
{
	[Inject]
	private IBackgroundTaskManagerService SpotifyTaskManagerService { get; set; } = default!;

	[Parameter]
	public required BackgroundTask SpotifyBackgroundTask { get; set; }

	[Parameter]
	public required TaskComponent ComponentType { get; set; }


	public bool CanDeleteTask => ComponentType == TaskComponent.Popup;

	private string DeleteOrHideButtonTitle => $"{(CanDeleteTask ? "Delete" : "Hide")} task";

	private string TaskClass => $"task {ComponentType.ToLowerString()} {(SpotifyBackgroundTask.Failed ? "failed" : "")} {(SpotifyBackgroundTask.Ended ? "finished" : "running")}";


	protected override void OnInitialized()
	{
		SpotifyTaskManagerService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		SpotifyTaskManagerService.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void DeleteOrHideTask()
	{
		if (CanDeleteTask)
		{
			DeleteTask();
			return;
		}

		HideTask();
	}

	private void HideTask()
	{
		SpotifyTaskManagerService.HideTask(SpotifyBackgroundTask);
	}

	private void DeleteTask()
	{
		SpotifyTaskManagerService.RemoveTask(SpotifyBackgroundTask);
	}
}
