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

	public ISet<SpotifyArtist> FilteredArtists
	{
		get
		{
			var artists = _spotifyReleaseFilterService.FilteredArtists;

			if (artists is null)
			{
				return new SortedSet<SpotifyArtist>();
			}

			var searched = artists.ApplySearch(SearchText, x => x.Name);

			var artistFilter = _spotifyReleaseFilterService.Filter.Artist;

			if (artistFilter is not null)
			{
				var extraArtist = artistFilter is null
					? []
					: artists.Where(x => x.Id == artistFilter);

				searched = searched.Union(extraArtist);
			}

			return new SortedSet<SpotifyArtist>(searched);
		}
	}

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
