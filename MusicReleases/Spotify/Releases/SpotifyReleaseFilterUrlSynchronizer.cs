using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.MusicReleases.Spotify.Releases.User;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Spotify.Releases;

internal class SpotifyReleaseFilterUrlSynchronizer : IDisposable, ISpotifyReleaseFilterUrlSynchronizer
{
	private readonly ISpotifyReleaseFilterService _filterService;

	private readonly ISpotifyReleaseFilterUrlService _filterUrlService;

	private readonly ISpotifyUserFilterReleaseDbService _dbService;


	private readonly NavigationManager _navManager;

	public SpotifyReleaseFilterUrlSynchronizer(ISpotifyReleaseFilterService filterService, ISpotifyReleaseFilterUrlService filterUrlService, ISpotifyUserFilterReleaseDbService dbService, NavigationManager navManager)
	{
		_filterService = filterService;
		_filterUrlService = filterUrlService;
		_dbService = dbService;
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


		// TODO cancel token
		await _dbService.Save(filter, false, default);

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

		// TODO cancel token
		var filter = await _dbService.Get(default) ?? new();
		_filterService.EnsureFilter(filter);


		ChangeUrl(filter);
		Console.WriteLine("SetInitFilter - end");
	}
}
