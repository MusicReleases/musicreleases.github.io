using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal sealed class SpotifyArtistFilterService : ISpotifyArtistFilterService
{
	private readonly ISpotifyReleaseFilterService _spotifyReleaseFilterService;

	public SpotifyArtistFilterService(ISpotifyReleaseFilterService spotifyReleaseFilterService)
	{
		_spotifyReleaseFilterService = spotifyReleaseFilterService;
		_spotifyReleaseFilterService.OnFilterChanged += DataChanged;
	}

	public void Dispose()
	{
		_spotifyReleaseFilterService.OnFilterChanged -= DataChanged;
		GC.SuppressFinalize(this);
	}

	public event Action? OnSearchTextChanged;
	public event Action? OnDataChanged;
	public event Action? OnChanged;

	public string? SearchText { get; private set; }


	public IReadOnlySet<SpotifyArtist>? FilteredArtists { get; private set; }

	private void DataChanged()
	{
		Recalculate();
		OnDataChanged?.Invoke();
		OnChanged?.Invoke();
	}

	private void SearchChanged()
	{
		Recalculate();
		OnDataChanged?.Invoke();
		OnSearchTextChanged?.Invoke();
		OnChanged?.Invoke();
	}

	private void Recalculate()
	{
		var artists = _spotifyReleaseFilterService.FilteredArtists;
		if (artists is null)
		{
			FilteredArtists = null;
			return;
		}

		var searched = artists.ApplySearch(SearchText, x => x.Name);

		var artistFilter = _spotifyReleaseFilterService.Filter.Artist;
		if (artistFilter is not null)
		{
			searched = searched.Union(artists.Where(x => x.Id == artistFilter));
		}

		FilteredArtists = new SortedSet<SpotifyArtist>(searched);
	}

	public void SetSearch(string? newSearchText)
	{
		newSearchText = newSearchText.EnsureText();

		if (string.Equals(newSearchText, SearchText, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		SearchText = newSearchText;
		SearchChanged();
	}
}
