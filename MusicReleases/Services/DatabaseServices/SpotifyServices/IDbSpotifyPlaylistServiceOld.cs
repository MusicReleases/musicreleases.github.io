using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices
{
	public interface IDbSpotifyPlaylistServiceOld
	{
		Task<ISet<SpotifyPlaylist>?> GetAll();
		Task Save(ISet<SpotifyPlaylist> playlists);
	}
}