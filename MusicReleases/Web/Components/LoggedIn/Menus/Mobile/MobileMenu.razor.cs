using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Mobile;

public partial class MobileMenu
{
	[Parameter]
	public EventCallback<bool> OnMoreClick { get; set; }


	private void DisplayMore(bool showMore)
	{
		OnMoreClick.InvokeAsync(showMore);
	}
}
