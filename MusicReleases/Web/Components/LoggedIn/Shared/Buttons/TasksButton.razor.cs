using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Shared.Buttons;

public partial class TasksButton : IDisposable
{
	[Inject]
	private IPopupService PopupService { get; set; } = default!;

	[Inject]
	private IOverflowMenuService OverflowMenuService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required TasksButtonComponent ButtonType { get; set; }

	[Parameter]
	public string? Class { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }


	private bool IsPopupDisplayed => PopupService.IsPopupDisplayed(_popupType);

	private string? ButtonText => ChildContent is null ? "Tasks" : null;

	private string ButtonTitle => $"{(IsPopupDisplayed ? "Show" : "Hide")} tasks";

	private string ButtonClass => $"tasks {Class}";


	private const PopupType _popupType = PopupType.Tasks;


	protected override void OnInitialized()
	{
		PopupService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		PopupService.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void ViewTasks()
	{
		OverflowMenuService.HideMenu();
		PopupService.Toggle(_popupType);
	}
}
