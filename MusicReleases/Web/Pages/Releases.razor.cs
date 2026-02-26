using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Releases : IDisposable
{
	[Inject]
	private ISpotifyWorkflowService SpotifyWorkflowService { get; set; } = default!;

	[Inject]
	private IApiLoginService ApiLoginService { get; set; } = default!;

	[Inject]
	private ISpotifyFilterUrlService SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	private ISpotifyFilterService SpotifyFilterService { get; set; } = default!;

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

	// this names must be same as in the URL and in Enums.ReleasesFilters
	[Parameter]
	[SupplyParameterFromQuery]
	public string? Tracks { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? EPs { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? NotRemixes { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? Remixes { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? FollowedArtists { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? SavedReleases { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? NotVariousArtists { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? VariousArtists { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? NewReleases { get; set; }

	[Parameter]
	[SupplyParameterFromQuery]
	public string? OldReleases { get; set; }


	private ReleaseType _releaseType;


	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	protected override async Task OnParametersSetAsync()
	{
		await LoadReleases();
	}

	private async Task LoadReleases()
	{
		if (PopupService.UrlChanged())
		{
			await LoadFiter();
			await GetReleases();
			// when is the same url as when the popup was opened - dont update
		}
	}

	private async Task LoadFiter()
	{
		// TODO enable to select and display more than 1 release type
		/*if (string.IsNullOrEmpty(Type))
		{
			// TODO display all releases and remember last selection
			//navManager.NavigateTo("/releases/albums");
			// TODO if is return here, code doesnt refresh the content
			// TODO but if is not here, code just continue and doesnt get the right Type (for example)
			return;
		}*/
		var tracks = IsFilterActive(Tracks);
		var eps = IsFilterActive(EPs);
		var notRemixes = IsFilterActive(NotRemixes);
		var remixes = IsFilterActive(Remixes);
		var followedArtists = IsFilterActive(FollowedArtists);
		var savedReleases = IsFilterActive(SavedReleases);
		var notVariousArtists = IsFilterActive(NotVariousArtists);
		var variousArtists = IsFilterActive(VariousArtists);
		var newReleases = IsFilterActive(NewReleases);
		var oldReleases = IsFilterActive(OldReleases);

		var advancedFilter = new SpotifyFilterAdvanced(tracks, eps, notRemixes, remixes, followedArtists, savedReleases, notVariousArtists, variousArtists, newReleases, oldReleases);
		var filter = SpotifyFilterUrlService.ParseFilterUrl(Type, Year, Month, ArtistId, advancedFilter);
		_releaseType = filter.ReleaseType;
		await SpotifyFilterService.SetFilterAndSaveDb(filter);
	}

	private static bool IsFilterActive(string? filter)
	{
		var active = filter is not null && (filter.IsNullOrEmpty() || bool.TryParse(filter, out var val) && val);
		return active;
	}

	private async Task GetReleases()
	{
		var userLoggedIn = ApiLoginService.IsUserLoggedIn();

		if (!userLoggedIn)
		{
			return;
		}

		var serviceType = ApiLoginService.GetServiceType();

		if (serviceType == ServiceType.Spotify)
		{
			// TODO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! loading every time from db - prefer loading from store !!!!!
			await SpotifyWorkflowService.StartLoadingAll(false, _releaseType);
		}
	}
}