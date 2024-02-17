namespace JakubKastner.MusicReleases.Web.Layouts;

public partial class LayoutLogin
{
	private bool _isUserSaved = true;

	// TODO login layout
	protected override async Task OnInitializedAsync()
	{
		await base.OnInitializedAsync();
		_isUserSaved = await _loginController.IsUserSaved();
		// check if user is logged in
		await _loginController.AutoLoginUser();
	}

}