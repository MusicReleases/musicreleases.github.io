using JakubKastner.MusicReleases.Services.BaseServices;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header;

public partial class Date
{
	private bool _displayTitle = true;
	private readonly MenuButtonsType _type = MenuButtonsType.Date;


	private bool Loading => LoaderService.IsLoading(LoadingType.Artists) || LoaderService.IsLoading(LoadingType.Releases);

	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += LoadingStateChanged;
		base.OnInitialized();
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= LoadingStateChanged;
	}

	private void LoadingStateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void DisplayTitle(bool displayTitle)
	{
		_displayTitle = displayTitle;
	}
}
