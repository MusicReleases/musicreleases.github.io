using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyReleasesServiceOld
{
	SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>? Releases { get; }

	event Action? OnTracksDataChanged;

	Task Get(MainReleasesType releaseType, ISet<SpotifyArtist> artists, bool forceUpdate);
}