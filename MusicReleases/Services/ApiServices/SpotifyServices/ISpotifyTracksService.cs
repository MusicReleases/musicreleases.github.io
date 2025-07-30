using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyTracksService
{
	SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? Releases { get; }

	event Action? OnTracksDataChanged;

	Task Get(SpotifyRelease release);
}