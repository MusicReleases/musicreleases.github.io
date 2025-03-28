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

	private SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? _releases => _stateSpotifyReleases.Value.List;
	private ISet<SpotifyRelease>? _releasesList => _releases?.List is null ? null : new SortedSet<SpotifyRelease>(_releases.List.Where(x => x.ReleaseType == _type));

	private bool _error => _stateSpotifyReleases.Value.Error;
	private bool _loading => _stateSpotifyReleases.Value.LoadingAny();
	private bool _errorArtists => _stateSpotifyArtists.Value.Error;
	private bool _loadingArtists => _stateSpotifyArtists.Value.LoadingAny();

	protected override void OnInitialized()
	{
		base.OnInitialized();
		Console.WriteLine("Releases.OnInitialized");
		//LoadReleases();
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
