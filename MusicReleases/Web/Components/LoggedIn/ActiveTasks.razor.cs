using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn;

public partial class ActiveTasks
{
	private IEnumerable<SpotifyBackgroundTask> VisibleTasks => TaskManager.Tasks.Where(t => t.IsOverlayVisible);


	protected override void OnInitialized()
	{
		TaskManager.OnChange += HandleChange;
	}

	public void Dispose()
	{
		TaskManager.OnChange -= HandleChange;
	}

	private void RemoveTask(SpotifyBackgroundTask task)
	{
		TaskManager.Tasks.Remove(task);
	}

	private void HandleChange()
	{
		InvokeAsync(StateHasChanged);
	}
}
