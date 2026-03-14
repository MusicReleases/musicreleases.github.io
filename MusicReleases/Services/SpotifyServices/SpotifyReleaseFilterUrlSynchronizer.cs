using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.SpotifyApi.Services.Api;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public class SpotifyReleaseFilterUrlSynchronizer : IDisposable, ISpotifyReleaseFilterUrlSynchronizer
{
	private readonly ISpotifyReleaseFilterService _filterService;

	private readonly ISpotifyReleaseFilterUrlService _filterUrlService;

	private readonly IDbSpotifyUserFilterReleaseService _dbService;

	private readonly IApiUserClient _spotifyUserClient;

	private readonly NavigationManager _navManager;

	public SpotifyReleaseFilterUrlSynchronizer(ISpotifyReleaseFilterService filterService, ISpotifyReleaseFilterUrlService filterUrlService, IDbSpotifyUserFilterReleaseService dbService, IApiUserClient spotifyUserClient, NavigationManager navManager)
	{
		_filterService = filterService;
		_filterUrlService = filterUrlService;
		_dbService = dbService;
		_spotifyUserClient = spotifyUserClient;
		_navManager = navManager;

		_filterService.NotifySynchronizer += OnFilterChanged;
	}

	public void Dispose()
	{
		_filterService.NotifySynchronizer -= OnFilterChanged;
		GC.SuppressFinalize(this);
	}

	private const string _baseUrl = "/releases/";

	public async Task SetFilterFromUrl(string? releaseType, string? year, string? month, string? artist, string? advancedFilterParams, string? searchTextParam)
	{
		Console.WriteLine("SetFilterFromUrl - start");

		var filter = _filterUrlService.ParseFilterFromUrlParams(releaseType, year, month, artist, advancedFilterParams, searchTextParam);

		if (filter == _filterService.Filter)
		{
			// when the same filter is already set - dont update
			return;
		}

		_filterService.SetFromUrl(filter);

		var userId = _spotifyUserClient.GetUserIdRequired();

		await _dbService.Save(filter, userId);

		Console.WriteLine("SetFilterFromUrl - end");
	}

	private void OnFilterChanged()
	{
		ChangeUrl(_filterService.Filter);
	}

	private void ChangeUrl(SpotifyReleaseFilter filter)
	{
		var paramaters = _filterUrlService.CreateUrl(filter);
		var url = $"{_baseUrl}{paramaters}";
		Console.WriteLine($"Navigate to: {url}");
		_navManager.NavigateTo(url, false);
	}

	public async Task SetInitFilter()
	{
		Console.WriteLine("SetInitFilter - start");
		var userId = _spotifyUserClient.GetUserIdRequired();
		var filter = await _dbService.Get(userId) ?? new();
		_filterService.EnsureFilter(filter);


		ChangeUrl(filter);
		Console.WriteLine("SetInitFilter - end");
	}
}
