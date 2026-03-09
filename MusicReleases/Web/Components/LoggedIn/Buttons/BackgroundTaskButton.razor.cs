using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Buttons;

public partial class BackgroundTaskButton : IDisposable
{
	[Inject]
	private IPopupService PopupService { get; set; } = default!;

	[Inject]
	private IOverflowMenuService OverflowMenuService { get; set; } = default!;

	[Inject]
	private ISpotifyTaskManagerService SpotifyTaskManagerService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required TasksButtonComponent ButtonType { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private bool IsPopupDisplayed => PopupService.IsPopupDisplayed(_popupType);

	private string? ButtonText => ButtonType == TasksButtonComponent.Settings ? "Tasks" : null;

	private string ButtonTitle => $"{(IsPopupDisplayed ? "Hide" : "Show")} tasks";

	private string ButtonClass => $"tasks {Class}";

	private bool ButtonLoading => ButtonType == TasksButtonComponent.Mobile && SpotifyTaskManagerService.IsAnyTaskRunning;

	private string? IconClass => ButtonType == TasksButtonComponent.Mobile ? "fill" : null;


	private const PopupType _popupType = PopupType.BackgroundTasks;


	protected override void OnInitialized()
	{
		PopupService.OnChange += StateChanged;
		SpotifyTaskManagerService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		PopupService.OnChange -= StateChanged;
		SpotifyTaskManagerService.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task ViewTasks()
	{
		OverflowMenuService.HideMenu();
		await PopupService.Toggle(_popupType);
	}
}
