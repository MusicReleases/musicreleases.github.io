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
		BackgroundTaskManager.OnUiRelevantChange += StateChanged;
	}

	public void Dispose()
	{
		BackgroundTaskManager.OnUiRelevantChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task ViewTasks()
	{
		Console.WriteLine("toggle - tasks active");
		await PopupService.Toggle(PopupType.BackgroundTasks);
	}
}
