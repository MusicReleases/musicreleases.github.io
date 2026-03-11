using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public class SpotifyArtistFilterService : IDisposable, ISpotifyArtistFilterService
{
	private readonly ISpotifyReleaseFilterService _spotifyReleaseFilterService;

	public SpotifyArtistFilterService(ISpotifyReleaseFilterService spotifyReleaseFilterService)
	{
		_spotifyReleaseFilterService = spotifyReleaseFilterService;
		_spotifyReleaseFilterService.OnFilterChanged += SearchOrDataChanged;
	}

	public void Dispose()
	{
		_spotifyReleaseFilterService.OnFilterChanged -= SearchOrDataChanged;
		GC.SuppressFinalize(this);
	}

	private void SearchOrDataChanged()
	{
		OnSearchOrDataChanged?.Invoke();
	}

	public event Action? OnSearchOrDataChanged;

	public string? SearchText { get; private set; }

	public ISet<SpotifyArtist>? FilteredArtists => SearchText.IsNullOrEmpty() ? _spotifyReleaseFilterService.FilteredArtists : _spotifyReleaseFilterService.FilteredArtists?.Where(x => x.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase) || (_spotifyReleaseFilterService.Filter.Artist is not null && x.Id == _spotifyReleaseFilterService.Filter.Artist)).ToHashSet();

	public void SetSearch(string? newSearchText)
	{
		newSearchText = newSearchText.EnsureText();

		if (string.Equals(newSearchText, SearchText, StringComparison.OrdinalIgnoreCase))
		{
			return;
		}

		SearchText = newSearchText;
		SearchOrDataChanged();
	}
}
