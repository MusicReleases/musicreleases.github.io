using MusicReleases.Api.Spotify.Objects;

namespace MusicReleases.Api.Spotify;

public class Login
{
    private readonly Controller _controller;
    private readonly IUser _user;

    public Login(Controller controller, IUser user)
    {
        _controller = controller;
        _user = user;
    }

    public async Task LoginUser(string url)
    {
        await LoginUser(new Uri(url));
    }

    public async Task LoginUser(Uri url)
    {
        _controller.LoginUser(url);
        await _user.SetUser();
    }
}
