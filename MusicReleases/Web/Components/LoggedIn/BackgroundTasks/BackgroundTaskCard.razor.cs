using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.MusicReleases.Spotify.Tasks;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.BackgroundTasks;

public partial class BackgroundTaskCard : IDisposable
{
	[Inject]
	private IPopupService PopupService { get; set; } = default!;

	[Inject]
	private IBackgroundTaskManagerService BackgroundTaskManager { get; set; } = default!;

	[Parameter]
	public required BackgroundTask BackgroundTask { get; set; }


	private string TaskClass
	{
		get
		{
			var classes = new List<string> { "task" };

			if (BackgroundTask.Status == BackgroundTaskStatus.Failed)
			{
				classes.Add("failed");
			}

			if (BackgroundTask.Ended)
			{
				classes.Add("finished");
			}
			else
			{
				classes.Add("running");
			}

			return string.Join(" ", classes);
		}
	}

	protected override void OnInitialized()
	{
		BackgroundTaskManager.OnChange += StateChanged;
	}

	public void Dispose()
	{
		BackgroundTaskManager.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void HideTask()
	{
		BackgroundTaskManager.HideTask(BackgroundTask);
	}

	private async Task ViewTask()
	{
		await PopupService.Toggle(PopupType.BackgroundTasks);
	}
}
