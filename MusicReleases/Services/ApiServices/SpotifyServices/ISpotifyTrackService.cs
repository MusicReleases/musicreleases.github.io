using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyTrackService
{
	event Action? OnTracksDataChanged;

	Task Get(SpotifyRelease release);
}