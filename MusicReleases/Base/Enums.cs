namespace JakubKastner.MusicReleases.Base;

public static class Enums
{
	public enum ServiceType
	{
		Spotify,
	}

	public enum LocalStorageKey
	{
		UserInfo,
		UserCredentials,
		LoginVerifier,
	}


	public static string GetLocalStorageKey(ServiceType serviceType, LocalStorageKey localStorageKey)
	{
		var prefix = serviceType.ToString();
		var key = localStorageKey.ToString();

		return prefix + "_" + key;
	}
}