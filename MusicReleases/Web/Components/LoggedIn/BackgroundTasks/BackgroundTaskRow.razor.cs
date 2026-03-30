using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Spotify.Settings;
using JakubKastner.MusicReleases.Spotify.Tasks;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.BackgroundTasks;

public partial class BackgroundTaskRow : IDisposable
{
	[Inject]
	private IBackgroundTaskManagerService BackgroundTaskManager { get; set; } = default!;

	[Inject]
	private ISpotifySettingsService SettingsService { get; set; } = default!;


	[Parameter]
	public required BackgroundTask BackgroundTask { get; set; }

	private string Class
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

	public LucideIcon Icon => GetIcon(BackgroundTask.Status);

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
		BackgroundTaskManager.OnChange += StateChanged;
		SettingsService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		BackgroundTaskManager.OnChange -= StateChanged;
		SettingsService.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void DeleteTask()
	{
		BackgroundTaskManager.RemoveCompletedTask(BackgroundTask);
	}

	private void CancelTask()
	{
		BackgroundTask.RequestCancel();
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
