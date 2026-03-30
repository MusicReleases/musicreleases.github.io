using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.MusicReleases.Spotify.Tasks;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.BackgroundTasks;

public partial class ActiveBackgroundTasks : IDisposable
{
	[Inject]
	private IBackgroundTaskManagerService BackgroundTaskManager { get; set; } = default!;

	[Inject]
	private IPopupService PopupService { get; set; } = default!;

	private ICollection<BackgroundTask> BackgroundTasks => BackgroundTaskManager.VisibleTasks.ToList();


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

	private async Task ViewTasks()
	{
		await PopupService.Toggle(PopupType.BackgroundTasks);
	}
}
