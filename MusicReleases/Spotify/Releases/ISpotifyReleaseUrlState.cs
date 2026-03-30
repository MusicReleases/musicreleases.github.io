namespace JakubKastner.MusicReleases.Spotify.Releases;

internal interface ISpotifyReleaseUrlState
{
	SpotifyReleaseUrlParameters? LastParams { get; set; }
}