using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices
{
	public interface IDbSpotifyPlaylistService
	{
		Task<ISet<SpotifyPlaylist>?> GetAll();
		Task Save(ISet<SpotifyPlaylist> playlists);
	}
}