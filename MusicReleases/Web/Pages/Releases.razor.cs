using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Base;
using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Releases
{
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



	private ReleaseType _type;

	private ISet<SpotifyRelease>? FilteredReleases => SpotifyFilterService.FilteredReleases;
	private bool Loading => LoaderService.IsLoading(Enums.LoadingType.Releases);
	private bool LoadingArtists => LoaderService.IsLoading(Enums.LoadingType.Artists);


	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += LoadingStateChanged;
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;

		base.OnInitialized();

		Console.WriteLine("Releases.OnInitialized");
		//LoadReleases();
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= LoadingStateChanged;
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
		//GC.SuppressFinalize(this);
	}

	private void LoadingStateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
	private void OnFilterOrDataChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	protected override async Task OnParametersSetAsync()
	{
		// TODO https://stackoverflow.com/questions/54345380/executing-method-on-parameter-change
		Console.WriteLine("Releases.OnParametersSet");
		await LoadReleases();
	}

	private async Task LoadReleases()
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
		_type = filter.ReleaseType;
		await SpotifyFilterService.SetFilterAndSaveDb(filter);
		GetReleases();
	}

	private static bool IsFilterActive(string? filter)
	{
		var active = filter is not null && (filter.IsNullOrEmpty() || bool.TryParse(filter, out var val) && val);
		return active;
	}

	private void GetReleases()
	{
		Console.WriteLine("get releases -------------");

		var userLoggedIn = ApiLoginService.IsUserLoggedIn();

		if (!userLoggedIn)
		{
			return;
		}

		var serviceType = ApiLoginService.GetServiceType();

		if (serviceType == Enums.ServiceType.Spotify)
		{
			SpotifyWorkflowService.StartLoadingAll(false, _type);
		}
	}
}