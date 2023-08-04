using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public interface IControllerApiTrack
{
	Task<IList<SpotifyTrack>> GetPlaylistTracksFromApi(string playlistId);
}