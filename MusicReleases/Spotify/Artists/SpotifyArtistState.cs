using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal sealed class SpotifyArtistState : ISpotifyArtistState
{
	public event Action? OnChange;

	public IReadOnlySet<SpotifyArtist>? FollowedArtists { get; private set; }

	public DateTime? LastSync { get; private set; }


	public void SetFollowed(IReadOnlyCollection<SpotifyArtist> artists, DateTime lastSync)
	{
		FollowedArtists = new SortedSet<SpotifyArtist>(artists);
		LastSync = lastSync;

		StateChanged();
	}

	public bool IsFollowed(string artistId)
	{
		if (FollowedArtists is null)
		{
			return false;
		}

		return FollowedArtists.Any(a => a.Id == artistId);
	}

	private void StateChanged() => OnChange?.Invoke();
}