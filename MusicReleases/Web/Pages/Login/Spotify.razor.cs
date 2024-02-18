using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Web.Pages.Login;

public partial class Spotify
{
	private bool _loading = true;
	private bool _error = false;

	protected override async Task OnInitializedAsync()
	{
		_loading = true;
		_error = false;

		await LoadPage();
		await base.OnInitializedAsync();

		_loading = false;
	}

	private async Task LoadPage()
	{
		// TODO global service type

		var uri = new Uri(_navManager.Uri);
		var uriQuery = uri.Query;

		var queryParams = QueryHelpers.ParseQuery(uriQuery);
		if (!queryParams.TryGetValue("code", out StringValues code))
		{
			// display message and login buttons
			_error = true;
			return;
		}

		await _loginController.SetUser(code);
	}
}