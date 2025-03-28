using JakubKastner.MusicReleases.Base;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using Microsoft.AspNetCore.Components;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Pages;

public partial class Releases
{
	[Parameter]
	public string? Type { get; set; }

	private ReleaseType _type;

	private SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? ReleasesUserList => SpotifyReleaseState.Value.List;
	private ISet<SpotifyRelease>? ReleasesList => ReleasesUserList?.List is null ? null : new SortedSet<SpotifyRelease>(ReleasesUserList.List.Where(x => x.ReleaseType == _type));

	private bool Error => SpotifyReleaseState.Value.Error;
	private bool Loading => SpotifyReleaseState.Value.LoadingAny();
	private bool ErrorArtists => SpotifyArtistState.Value.Error;
	private bool LoadingArtists => SpotifyArtistState.Value.LoadingAny();

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
		FilterService.FilterReleaseType(_type);
		GetReleases();
	}

	private void GetReleases()
	{
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
