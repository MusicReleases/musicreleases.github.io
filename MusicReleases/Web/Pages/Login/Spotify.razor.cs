using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Web.Pages.Login;

public partial class Spotify
{
	[Inject]
	private ILoginService LoginService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	private bool _loading = true;

	private bool _error = false;


	protected override async Task OnInitializedAsync()
	{
		_loading = true;
		_error = false;

		try
		{
			await LoadPage();
		}
		finally
		{
			_loading = false;
		}
	}

	private async Task LoadPage()
	{
		// TODO global service type

		var uri = new Uri(NavManager.Uri);
		var uriQuery = uri.Query;

		var queryParams = QueryHelpers.ParseQuery(uriQuery);
		if (!queryParams.TryGetValue("code", out StringValues code))
		{
			// display message and login buttons
			_error = true;
			return;
		}

		await LoginService.SetUser(code);
	}
}