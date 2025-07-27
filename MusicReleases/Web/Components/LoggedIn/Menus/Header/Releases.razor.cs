using JakubKastner.MusicReleases.Base;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header;

public partial class Releases
{
	private bool Loading => LoaderService.IsLoading(Enums.LoadingType.Artists) || LoaderService.IsLoading(Enums.LoadingType.Releases);
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
}
