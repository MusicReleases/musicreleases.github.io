using JakubKastner.MusicReleases.Base;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;
using static JakubKastner.MusicReleases.Store.FilterStore.SpotifyFilterAction;
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

	private ReleaseType _type;

	/*private SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? ReleasesUserList => SpotifyReleaseState.Value.List;
	private ISet<SpotifyRelease>? ReleasesList => ReleasesUserList?.List is null ? null : new SortedSet<SpotifyRelease>(ReleasesUserList.List.Where(x => x.ReleaseType == _type));*/

	private ISet<SpotifyRelease>? FilteredReleases => SpotifyFilterService.FilteredReleases;

	private bool Error => SpotifyReleaseState.Value.Error;
	private bool Loading => SpotifyReleaseState.Value.LoadingAny();
	private bool ErrorArtists => SpotifyArtistState.Value.Error;
	private bool LoadingArtists => SpotifyArtistState.Value.LoadingAny();


	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;

		base.OnInitialized();

		Console.WriteLine("Releases.OnInitialized");
		//LoadReleases();
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
	}

	protected override void OnParametersSet()
	{
		// TODO https://stackoverflow.com/questions/54345380/executing-method-on-parameter-change
		Console.WriteLine("Releases.OnParametersSet");
		LoadReleases();
	}

	private void LoadReleases()
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

		var filter = SpotifyFilterUrlService.ParseFilterUrl(Type, Year, Month, ArtistId);
		_type = filter.ReleaseType;

		Dispatcher.Dispatch(new SetFiltersAction(filter));
		GetReleases();
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

	private void OnFilterOrDataChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}