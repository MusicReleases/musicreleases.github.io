using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Tasks;

public partial class ActiveTasks : IDisposable
{
	[Inject]
	private ISpotifyTaskManagerService SpotifyTaskManagerService { get; set; } = default!;

	[Inject]
	private IPopupService PopupService { get; set; } = default!;


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

	private void ViewTasks()
	{
		PopupService.Toggle(PopupType.Tasks);
	}
}
