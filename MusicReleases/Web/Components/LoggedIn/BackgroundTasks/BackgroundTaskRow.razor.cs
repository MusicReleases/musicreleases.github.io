using JakubKastner.MusicReleases.Enums;
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

	private string Class => $"task {(SpotifyBackgroundTask.Failed ? "failed" : string.Empty)} {(SpotifyBackgroundTask.Ended ? "finished" : "running")}";

	public LucideIcon Icon => GetIcon(SpotifyBackgroundTask.Status);

	private static LucideIcon GetIcon(BackgroundTaskStatus status)
	{
		return status switch
		{
			BackgroundTaskStatus.Running => LucideIcon.LoaderCircle,
			BackgroundTaskStatus.Failed => LucideIcon.CircleAlert,
			BackgroundTaskStatus.Finished => LucideIcon.CircleCheckBig,
			BackgroundTaskStatus.Canceled => LucideIcon.Ban,
			_ => LucideIcon.Dot
		};
	}

	private const string _iconClass = "task-small";
	private const string _buttonClass = "task-row";

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
	private void CancelTask()
	{
		SpotifyBackgroundTask.RequestCancel();
	}

	private string GetButtonUrl(BackgroundTaskLink link)
	{
		if (link.UrlApp.IsNullOrEmpty())
		{
			return link.UrlWeb;
		}

		return SettingsService.GetUrl(link.UrlApp, link.UrlWeb);
	}

	private string GetButtonText(BackgroundTaskLink link)
	{
		if (link.UrlApp.IsNullOrEmpty())
		{
			return link.Text;
		}

		return $"Open {link.Text}";
	}

	private string GetButtonTitle(BackgroundTaskLink link)
	{
		if (link.UrlApp.IsNullOrEmpty())
		{
			return link.Title;
		}

		return SettingsService.GetUrlTitle(link.Title);
	}
}
