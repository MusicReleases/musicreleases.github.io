namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Index
{
	private bool _loading = true;

	protected override async Task OnInitializedAsync()
	{
		_loading = true;

		await LoadPage();
		await base.OnInitializedAsync();

		_loading = false;
	}

	private async Task LoadPage()
	{
		// check if user is logged in
		await _loginController.AutoLoginUser();
	}
}
