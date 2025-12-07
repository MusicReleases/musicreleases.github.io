using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.State.Spotify
{
	public interface ISpotifyArtistState
	{
		IReadOnlyList<SpotifyArtist> SortedFollowedArtists { get; }

		event Action? OnChange;

		SpotifyArtist? GetById(string id);
		void MergeCache(IEnumerable<SpotifyArtist> others);
		void SetFollowed(IEnumerable<SpotifyArtist> artists);
	}
}