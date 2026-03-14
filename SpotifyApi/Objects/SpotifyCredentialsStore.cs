namespace JakubKastner.SpotifyApi.Objects;

internal class SpotifyCredentialsStore : ISpotifyCredentialsStore
{
	private SpotifyUserCredentials? _credentials;

	public SpotifyUserCredentials? GetCredentials() => _credentials;
	public void SetCredentials(SpotifyUserCredentials credentials) => _credentials = credentials;
}
