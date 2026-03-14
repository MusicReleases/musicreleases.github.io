using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.BackgroundTasks;

public partial class BackgroundTaskCard : IDisposable
{
	[Inject]
	private IPopupService PopupService { get; set; } = default!;

	[Inject]
	private IBackgroundTaskManagerService SpotifyTaskManagerService { get; set; } = default!;

	[Parameter]
	public required BackgroundTask BackgroundTask { get; set; }


	private string TaskClass => $"task {(BackgroundTask.Failed ? "failed" : "")} {(BackgroundTask.Ended ? "finished" : "running")}";


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

	private void HideTask()
	{
		SpotifyTaskManagerService.HideTask(BackgroundTask);
	}

	private async Task ViewTask()
	{
		await PopupService.Toggle(PopupType.BackgroundTasks);
	}
}
