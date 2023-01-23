using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi;

public class Client
{
    private SpotifyClient? _spotifyClient;

    public void Init(string accessToken)
    {
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new ArgumentNullException(nameof(accessToken));
        }
        _spotifyClient = new(accessToken);
    }

    public bool IsInicialized()
    {
        return _spotifyClient != null;
    }

    public SpotifyClient GetClient()
    {
        if (!IsInicialized())
        {
            throw new InvalidOperationException("SpotifyClient is not inicialized.");
        }
        return _spotifyClient!;
    }
}
