using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.BackgroundTasks;

public partial class BackgroundTask : IDisposable
{
	[Inject]
	private ISpotifyTaskManagerService SpotifyTaskManagerService { get; set; } = default!;

	[Inject]
	private IPopupService PopupService { get; set; } = default!;


	[Parameter]
	public required SpotifyBackgroundTask SpotifyBackgroundTask { get; set; }

	[Parameter]
	public required TaskComponent ComponentType { get; set; }


	public bool CanDeleteTask => ComponentType == TaskComponent.Popup;

	private string DeleteOrHideButtonTitle => $"{(CanDeleteTask ? "Delete" : "Hide")} task";

	private string TaskClass => $"task {ComponentType.ToLowerString()} {(SpotifyBackgroundTask.Failed ? "failed" : "")} {(SpotifyBackgroundTask.IsRunning ? "running" : "finished")}";


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
		SpotifyBackgroundTask.IsOverlayVisible = false;
	}

	private void DeleteTask()
	{
		SpotifyTaskManagerService.RemoveTask(SpotifyBackgroundTask);
	}
}
