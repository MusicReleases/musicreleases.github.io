using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;
using JakubKastner.MusicReleases.Store.LoaderStore;
using JakubKastner.MusicReleases.Web.Components.InfiniteScrolling;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Releases
{
	[Parameter]
	public string? Type { get; set; }

	// TODO enable to select and display more than 1 release type
	private ReleaseType _type;

	private SortedSet<SpotifyAlbum>? _releases => _stateSpotifyReleases.Value.Releases;
	private bool _loading => _stateSpotifyReleases.Value.Loading;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (!_spotifyControllerUser.IsLoggedIn())
		{
			_navManager.NavigateTo("");
			return;
		}

		// TODO https://stackoverflow.com/questions/54345380/executing-method-on-parameter-change
		//GetParameter();

		if (_stateSpotifyReleases.Value.Initialized == false)
		{
			LoadReleases();
			_dispatcher.Dispatch(new SpotifyReleasesActionInitialized());
		}
	}

	private async Task<IEnumerable<SpotifyAlbum>> GetReleases(InfiniteScrollingItemsProviderRequest request)
	{
		await Task.Delay(0);
		if (_releases == null)
		{
			return new SortedSet<SpotifyAlbum>();
		}
		return _releases.Skip(request.StartIndex).Take(15);
	}

	private void LoadReleases()
	{
		_dispatcher.Dispatch(new SpotifyReleasesActionLoad());
	}

	private void Loader()
	{
		_dispatcher.Dispatch(new LoaderAction(true));
	}
	private void LoaderS()
	{
		_dispatcher.Dispatch(new LoaderAction(false));
	}

	private void SaveToStorage()
	{
		_dispatcher.Dispatch(new SpotifyArtistsActionStorageSet(_stateSpotifyArtists.Value));
	}
	protected override void OnParametersSet()
	{
		GetParameter();
	}

	private void GetParameter()
	{
		if (string.IsNullOrEmpty(Type))
		{
			// TODO display all releases and remember last selection
			//navManager.NavigateTo("/releases/albums");
			// TODO if is return here, code doesnt refresh the content
			// TODO but if is not here, code just continue and doesnt get the right Type (for example)
			return;
		}
		try
		{
			_type = Enum.Parse<ReleaseType>(Type);
		}
		catch
		{
			_type = ReleaseType.Albums;
		}
	}
}
