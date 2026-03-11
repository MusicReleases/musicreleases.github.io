using JakubKastner.MusicReleases.Objects.BackgroundTasks;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.BackgroundTasks;

public partial class BackgroundTaskRow : IDisposable
{
	[Inject]
	private IBackgroundTaskManagerService SpotifyTaskManagerService { get; set; } = default!;

	[Inject]
	private ISettingsService SettingsService { get; set; } = default!;


	[Parameter]
	public required BackgroundTask SpotifyBackgroundTask { get; set; }

	private string Class => $"task-row {(SpotifyBackgroundTask.Failed ? "failed" : string.Empty)} {(SpotifyBackgroundTask.Ended ? "finished" : "running")}";


	protected override void OnInitialized()
	{
		SpotifyTaskManagerService.OnChange += StateChanged;
		SettingsService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		SpotifyTaskManagerService.OnChange -= StateChanged;
		SettingsService.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}


	private void DeleteTask()
	{
		SpotifyTaskManagerService.RemoveTask(SpotifyBackgroundTask);
	}
}
