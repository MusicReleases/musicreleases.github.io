using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyReleasesService
{
	SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? Releases { get; }

	event Action? OnTracksDataChanged;

	Task Get(ReleaseType releaseType, ISet<SpotifyArtist> artists, bool forceUpdate);
}