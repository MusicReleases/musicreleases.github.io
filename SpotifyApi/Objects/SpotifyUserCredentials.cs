namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserCredentials
{
	public string? RefreshToken { get; set; }

	public SpotifyUserCredentials() { }

	public SpotifyUserCredentials(string refreshToken)
	{
		RefreshToken = refreshToken;
	}
}
