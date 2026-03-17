using JakubKastner.SpotifyApi.Artists;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal interface ISpotifyArtistFilterService : IDisposable
{
	IReadOnlySet<SpotifyArtist>? FilteredArtists { get; }
	string? SearchText { get; }

	event Action? OnSearchTextChanged;
	event Action? OnChanged;

	void SetSearch(string? newSearchText);
}