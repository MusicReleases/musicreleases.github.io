using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using static JakubKastner.MusicReleases.Base.Enums;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class MenuDateYear
{
	private bool Loading => SpotifyArtistState.Value.LoadingAny() || SpotifyReleaseState.Value.LoadingAny();
	private SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? ReleasesUserList => SpotifyReleaseState.Value.List;

	private ISet<SpotifyRelease>? ReleasesList =>
	ReleasesUserList?.List is null
		? null
		: new SortedSet<SpotifyRelease>(ReleasesUserList.List.Where(x => x.ReleaseType == ReleaseType));

	private ISet<DateTime>? DateList =>
		ReleasesList is null
			? null
			: new SortedSet<DateTime>(ReleasesList.Select(x => x.ReleaseDate));

	private Dictionary<int, SortedSet<int>>? YearMonthList =>
		DateList?
			.Select(x => x.Year)
			.Distinct()
			.OrderByDescending(x => x)
			.ToDictionary(
				year => year,
				year => new SortedSet<int>(
					DateList.Where(d => d.Year == year).Select(d => d.Month),
					Comparer<int>.Create((x, y) => y.CompareTo(x))
				)
			);

	public ReleaseType ReleaseType => SpotifyFilterState.Value.Filter.ReleaseType;


	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (!ApiLoginService.IsUserLoggedIn())
		{
			return;
		}

		var serviceType = ApiLoginService.GetServiceType();
		if (serviceType == ServiceType.Spotify)
		{
			// TODO show loader
			// display playlists
			//_artists = await _spotifyControllerArtist.GetUserFollowedArtists();
		}
	}
}
