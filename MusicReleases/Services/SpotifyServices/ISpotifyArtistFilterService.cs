using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.SpotifyServices
{
	public interface ISpotifyArtistFilterService
	{
		ISet<SpotifyArtist>? FilteredArtists { get; }
		string? SearchText { get; }

		event Action? OnSearchOrDataChanged;

		void SetSearch(string? newSearchText);
	}
}