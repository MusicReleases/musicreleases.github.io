using Microsoft.AspNetCore.Components;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedOut;

public partial class Login
{
	/// <summary>
	/// Service type (Spotify, Apple Music, ...)
	/// </summary>
	[Parameter]
	public ServiceType Type { get; set; }

	private string _type = string.Empty;

	protected override async Task OnInitializedAsync()
	{
		_type = Type.ToString();

		// check if user is logged in
		await AutoLoginUser();
	}

	/// <summary>
	/// User clicked to login button.
	/// </summary>
	private void LoginUser()
	{
		_loginController.LoginUser(Type);
	}

	private async Task AutoLoginUser()
	{
		var savedUser = await _loginController.IsUserSaved(Type);
		if (!savedUser)
		{
			return;
		}

		LoginUser();
	}
}