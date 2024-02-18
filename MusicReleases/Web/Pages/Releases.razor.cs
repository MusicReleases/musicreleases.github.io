using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;
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

	private SortedSet<SpotifyRelease>? _releases => _stateSpotifyReleases.Value.Releases;
	private bool _loading => _stateSpotifyReleases.Value.Loading;

	protected override void OnParametersSet()
	{
		// TODO https://stackoverflow.com/questions/54345380/executing-method-on-parameter-change
		LoadReleases();
	}

	private async Task<IEnumerable<SpotifyRelease>> GetReleases(InfiniteScrollingItemsProviderRequest request)
	{
		await Task.Delay(0);
		if (_releases == null)
		{
			return new SortedSet<SpotifyRelease>();
		}
		return _releases.Skip(request.StartIndex).Take(15);
	}

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

		// TODO loading & loaded
		/*if (_stateSpotifyReleases.Value.Initialized == false)
		{*/
		_dispatcher.Dispatch(new SpotifyReleasesActionLoad(_type));
		_dispatcher.Dispatch(new SpotifyReleasesActionInitialized());
		/*}*/
	}

	private void SaveToStorage()
	{
		_dispatcher.Dispatch(new SpotifyArtistsActionStorageSet(_stateSpotifyArtists.Value));
	}
}
