using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class ReleaseMenuList
{
	[Parameter, EditorRequired]
	public MainMenuType MenuType { get; set; }

	[Parameter]
	public EventCallback<bool> OnMoreClick { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private string TitleMore => _showMore ? "Hide more" : "Show more";

	private string ClassMore => $"{_defaultButtonClass}{ActiveClass}";

	private string ActiveClass => _showMore ? " active" : string.Empty;

	private string ClassList => $"{MenuType.ToString().ToLower()} {Class}";

	private string ReleaseButtonClass => MenuType == MainMenuType.Primary ? "main-menu" : string.Empty;

	private LucideIcon IconMore => _showMore ? LucideIcon.X : LucideIcon.Ellipsis;


	private readonly string _defaultButtonClass = "main-menu rounded-xl fill-width trasparent";

	private bool _showMore = false;


	public void DisplayMore()
	{
		_showMore = !_showMore;

		OnMoreClick.InvokeAsync(_showMore);
	}
}
