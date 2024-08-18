using JakubKastner.MusicReleases.Base;
using Microsoft.AspNetCore.Components;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Releases
{
	[Parameter]
	public string? Type { get; set; }

	private ReleaseType _type;


	protected override void OnInitialized()
	{
		base.OnInitialized();
		LoadReleases();
	}

	private void GetReleases()
	{
		var userLoggedIn = _apiLoginController.IsUserLoggedIn();

		if (!userLoggedIn)
		{
			return;
		}

		var serviceType = _apiLoginController.GetServiceType();

		if (serviceType == Enums.ServiceType.Spotify)
		{
			_spotifyWorkflowController.StartLoadingAll(false, _type);
		}
	}

















	// TODO enable to select and display more than 1 release type

	//private SortedSet<SpotifyRelease>? _releases => /*_spotifyReleasesController.Releases;*/ _stateSpotifyReleases.Value.Releases;
	//private bool _loading => /*_spotifyReleasesController.Loading; */ _stateSpotifyReleases.Value.Loading;

	protected override void OnParametersSet()
	{
		// TODO https://stackoverflow.com/questions/54345380/executing-method-on-parameter-change
		LoadReleases();
	}

	/*private async Task<IEnumerable<SpotifyRelease>> GetReleases(InfiniteScrollingItemsProviderRequest request)
	{
		await Task.Delay(0);
		if (_releases == null)
		{
			return new SortedSet<SpotifyRelease>();
		}
		return _releases.Skip(request.StartIndex).Take(15);
	}*/

	private void LoadReleases()
	{
		if (string.IsNullOrEmpty(Type))
		{
			// TODO display all releases and remember last selection
			//navManager.NavigateTo("/releases/albums");
			// TODO if is return here, code doesnt refresh the content
			// TODO but if is not here, code just continue and doesnt get the right Type (for example)
			return;
		}

		if (!Enum.TryParse(Type, true, out _type))
		{
			_type = ReleaseType.Albums;
		}

		GetReleases();

		// TODO 010324
		//_spotifyReleasesController.LoadReleases(_type);

		// TODO loading & loaded
		/*if (_stateSpotifyReleases.Value.Initialized == false)
		{*/
		/*_dispatcher.Dispatch(new SpotifyReleasesActionLoad(_type));
		_dispatcher.Dispatch(new SpotifyReleasesActionInitialized());*/
		/*}*/
	}

	private void SaveToStorage()
	{
		//_spotifyReleasesController.SaveReleasesToStorage();
		//_dispatcher.Dispatch(new SpotifyArtistsActionStorageSet(_stateSpotifyArtists.Value));
	}
}
