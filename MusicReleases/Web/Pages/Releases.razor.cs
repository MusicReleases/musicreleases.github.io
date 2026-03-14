using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.SpotifyApi.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Releases
{
	[Inject]
	private ISpotifyWorkflowService SpotifyWorkflowService { get; set; } = default!;

	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;

	[Inject]
	private ISpotifyReleaseFilterUrlSynchronizer SpotifyReleaseFilterUrlSynchronizer { get; set; } = default!;

	[Inject]
	private IPopupService PopupService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	[Parameter]
	public string? Type { get; set; }

	[Parameter]
	public string? Year { get; set; }

	[Parameter]
	public string? Month { get; set; }

	[Parameter]
	public string? ArtistId { get; set; }


	[Parameter]
	[SupplyParameterFromQuery]
	public string? Filter { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? Search { get; set; }


	private ReleaseGroup? _lastReleaseType = null;

	protected override async Task OnParametersSetAsync()
	{
		Console.WriteLine(NavManager.Uri);
		await Load();
	}

	private async Task Load()
	{
		var urlChanged = await PopupService.UrlChanged();
		if (!urlChanged)
		{
			Console.WriteLine("url doesnt changed");
			// when is the same url as when the popup was opened - dont update
			return;
		}

		var loadReleases = await LoadFiter();

		if (loadReleases)
		{
			await LoadReleases();
		}
	}

	private async Task<bool> LoadFiter()
	{
		if (Type.IsNullOrEmpty())
		{
			await SpotifyReleaseFilterUrlSynchronizer.SetInitFilter();
			return false;
		}

		await SpotifyReleaseFilterUrlSynchronizer.SetFilterFromUrl(Type, Year, Month, ArtistId, Filter, Search);

		// type doesnt changed - dont update
		var currentReleaseType = SpotifyReleaseFilterService.Filter.ReleaseGroup;
		var typeChanged = _lastReleaseType != currentReleaseType;
		_lastReleaseType = currentReleaseType;

		return typeChanged;
	}

	private async Task LoadReleases()
	{
		Console.WriteLine("loading releases");
		await SpotifyWorkflowService.StartLoadingAll(SpotifyReleaseFilterService.Filter.ReleaseGroup, false);
	}
}