using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify;

public partial class Controller
{
    public async Task<PrivateUser?> GetUser()
    {
        if (SpotifyClient == null) return null;
        return await SpotifyClient.UserProfile.Current();
    }
}
