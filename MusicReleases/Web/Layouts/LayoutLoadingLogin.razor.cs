using Microsoft.AspNetCore.WebUtilities;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Layouts;

public partial class LayoutLoadingLogin
{
	// TODO loading page
	protected override async Task OnInitializedAsync()
	{
		// TODO global service type
		var uri = new Uri(_navManager.Uri);
		var queryParams = QueryHelpers.ParseQuery(uri.Query);
		if (queryParams.ContainsKey("code"))
		{
			var code = queryParams["code"];
			await _loginController.SetUser(ServiceType.Spotify, code);
		}
	}
}