namespace JakubKastner.MusicReleases.Web.Layouts;

public partial class LayoutMain
{
	protected override void OnInitialized()
	{
		MobileService.OnDisplayChanged += OnDisplayChanged;

		if (!CheckLoggedInUser())
		{
			return;
		}
		base.OnInitialized();
	}

	public void Dispose()
	{
		MobileService.OnDisplayChanged -= OnDisplayChanged;
	}

	private void OnDisplayChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private bool CheckLoggedInUser()
	{
		if (!ApiLoginService.IsUserLoggedIn())
		{
			NavManager.NavigateTo("");
			return false;
		}
		return true;
	}
}