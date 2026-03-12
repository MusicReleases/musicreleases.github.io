using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.State.Spotify;

public class SpotifyArtistState : ISpotifyArtistState
{
	public event Action? OnChange;

	public IReadOnlyList<SpotifyArtist> SortedFollowedArtists { get; private set; } = [];

	public DateTime? LastSync { get; private set; }


	public void SetFollowed(IEnumerable<SpotifyArtist> artists, DateTime lastSync)
	{
		var list = artists.OrderBy(a => a.Name).ToList();
		SortedFollowedArtists = list.AsReadOnly();
		LastSync = lastSync;

		NotifyStateChanged();
	}

	public bool IsFollowed(string artistId)
	{
		return SortedFollowedArtists.Any(a => a.Id == artistId);
	}

	private void NotifyStateChanged() => OnChange?.Invoke();
}