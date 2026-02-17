using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.UiServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Sidebars;

public partial class SidebarSection : IDisposable
{
	[Inject]
	private IMobileService MobileService { get; set; } = default!;


	[Parameter, EditorRequired]
	public SidebarType Type { get; set; }

	[Parameter, EditorRequired]
	public bool Loading { get; set; }

	[Parameter, EditorRequired]
	public RenderFragment Header { get; set; }

	[Parameter, EditorRequired]
	public RenderFragment Filter { get; set; }

	[Parameter, EditorRequired]
	public RenderFragment ChildContent { get; set; }

	[Parameter]
	public string? ContentAriaLabel { get; set; }


	private string TypeString => Type.ToString().ToLower();

	private MobileMenu? MobileMenuType => Type switch
	{
		SidebarType.Artists => MobileMenu.Artists,
		SidebarType.Date => MobileMenu.Date,
		_ => null
	};

	private string ClassShow => MobileService.MobileMenu == MobileMenuType ? "show" : string.Empty;

	protected override void OnInitialized()
	{
		MobileService.OnDisplayChanged += StateChanged;
	}

	public void Dispose()
	{
		MobileService.OnDisplayChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
