using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices
{
	public interface ISpotifyArtistsService
	{
		SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? Artists { get; }

		event Action? OnArtistsDataChanged;

		Task Get(bool forceUpdate);
	}
}