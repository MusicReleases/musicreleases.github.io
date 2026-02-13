using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Index
{
	[Inject]
	private ILoginService LoginService { get; set; } = default!;


	private bool _loading = true;


	protected override async Task OnInitializedAsync()
	{
		_loading = true;

		await LoadPage();

		_loading = false;
	}

	private async Task LoadPage()
	{
		// check if user is logged in
		await LoginService.AutoLoginUser();
	}
}
