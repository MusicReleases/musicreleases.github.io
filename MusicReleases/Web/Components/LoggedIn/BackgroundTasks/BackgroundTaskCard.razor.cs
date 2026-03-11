using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.BackgroundTasks;
using JakubKastner.MusicReleases.Services.SpotifyServices;
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
	public required BackgroundTask SpotifyBackgroundTask { get; set; }


	private string TaskClass => $"task {(SpotifyBackgroundTask.Failed ? "failed" : "")} {(SpotifyBackgroundTask.Ended ? "finished" : "running")}";


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
		SpotifyTaskManagerService.HideTask(SpotifyBackgroundTask);
	}

	private async Task ViewTask()
	{
		await PopupService.Toggle(PopupType.BackgroundTasks);
	}
}
