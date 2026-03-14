namespace JakubKastner.SpotifyApi.Objects;

public interface ISpotifyCredentialsStore
{
	SpotifyUserCredentials? GetCredentials();
	void SetCredentials(SpotifyUserCredentials credentials);
}