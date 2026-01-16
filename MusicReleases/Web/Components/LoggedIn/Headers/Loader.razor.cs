namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Headers;

public partial class Loader
{
	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += LoadingStateChanged;
	}

	private void LoadingStateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= LoadingStateChanged;
		GC.SuppressFinalize(this);
	}
}
