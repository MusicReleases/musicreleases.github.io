using JakubKastner.MusicReleases.Base;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Releases
{
	[Parameter]
	public string? Type { get; set; }

	private ReleaseType _type;

	private SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>? _artists => _stateSpotifyArtists.Value.List;
	private IEnumerable<SpotifyArtist>? _artistWithReleaseType => _artists?.List is null ? null : _artists.List.Where(x => x.Releases is not null && x.Releases.Any(y => y.ReleaseType == _type));
	private ISet<SpotifyRelease>? _releases => (_artistWithReleaseType is null || !_artistWithReleaseType.Any()) ? null : new SortedSet<SpotifyRelease>(_artistWithReleaseType.SelectMany(x => x.Releases!.Where(y => y.ReleaseType == _type/* && y.ReleaseDate.Year == DateTime.Now.Year*/)));
	private bool _error => _stateSpotifyReleases.Value.Error;
	private bool _loading => _stateSpotifyReleases.Value.LoadingAny();
	private bool _errorArtists => _stateSpotifyArtists.Value.Error;
	private bool _loadingArtists => _stateSpotifyArtists.Value.LoadingAny();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		LoadReleases();
	}


	protected override void OnParametersSet()
	{
		// TODO https://stackoverflow.com/questions/54345380/executing-method-on-parameter-change
		LoadReleases();
	}

	private void LoadReleases()
	{
		// TODO enable to select and display more than 1 release type
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
}
