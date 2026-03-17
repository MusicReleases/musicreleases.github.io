using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.SpotifyApi.Artists;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal sealed class SpotifyArtistFilterService : ISpotifyArtistFilterService
{
	private readonly ISpotifyReleaseFilterService _releaseFilterService;

	public SpotifyArtistFilterService(ISpotifyReleaseFilterService releaseFilterService)
	{
		_releaseFilterService = releaseFilterService;
		_releaseFilterService.OnFilterChanged += DataChanged;
	}

	public void Dispose()
	{
		_releaseFilterService.OnFilterChanged -= DataChanged;
		GC.SuppressFinalize(this);
	}

	public event Action? OnSearchTextChanged;
	public event Action? OnChanged;

	public string? SearchText { get; private set; }

	public IReadOnlySet<SpotifyArtist>? FilteredArtists { get; private set; }

	private void DataChanged()
	{
		Recalculate();

		OnChanged?.Invoke();
	}

	private void SearchChanged()
	{
		Recalculate();

		OnSearchTextChanged?.Invoke();
		OnChanged?.Invoke();
	}

	private void Recalculate()
	{
		var artists = _releaseFilterService.FilteredArtists;
		if (artists is null)
		{
			FilteredArtists = null;
			return;
		}

		var searched = artists.ApplySearch(SearchText, x => x.Name);

		// add current filtered artist
		var artistFilter = _releaseFilterService.Filter.Artist;
		if (artistFilter is not null)
		{
			searched = searched.Union(artists.Where(x => x.Id == artistFilter));
		}

		FilteredArtists = new SortedSet<SpotifyArtist>(searched);
	}

	public void SetSearch(string? searchText)
	{
		searchText = searchText.EnsureText();

		if (string.Equals(searchText, SearchText, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		SearchText = searchText;
		SearchChanged();
	}
}
