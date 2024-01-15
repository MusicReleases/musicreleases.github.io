namespace JakubKastner.MusicReleases.Base;

public static class Enums
{
	public enum ServiceType
	{
		Spotify,
	}

	public enum Theme
	{
		System,
		Dark,
		Light,
		Color,
	}

	public enum PlaylistVisiblity
	{
		Default,
		Public,
		Private,
	}

	public enum ReleasesFilters
	{
		Clear,
		Tracks,
		EPs,
		Remixes,
		FollowedArtists,
		VariousArtists,
		InLibrary,
		OnlyNew,
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
		var separator = "_";

		return prefix + separator + key;
	}
}