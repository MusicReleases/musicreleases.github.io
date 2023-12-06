using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
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
		if (!queryParams.TryGetValue("code", out StringValues code))
		{
			return;
		}
		await _loginController.SetUser(ServiceType.Spotify, code);
	}
}