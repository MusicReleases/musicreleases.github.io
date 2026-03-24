using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Tracks;

public interface ISpotifyTrackService
{
	event Action? OnTracksDataChanged;

	Task Get(SpotifyRelease release);
}