namespace JakubKastner.MusicReleases.Spotify.Releases;

internal sealed class SpotifyReleaseUrlState : ISpotifyReleaseUrlState
{
	public SpotifyReleaseUrlParameters? LastParams { get; set; }
}
