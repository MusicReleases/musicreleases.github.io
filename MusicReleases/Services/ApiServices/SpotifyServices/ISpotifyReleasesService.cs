using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices
{
	public interface ISpotifyReleasesService
	{
		SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? Releases { get; }

		event Action? OnTracksDataChanged;

		Task Get(SpotifyEnums.ReleaseType releaseType, ISet<SpotifyArtist> artists, bool forceUpdate);
	}
}