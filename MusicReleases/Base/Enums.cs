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

	// this names must be same as in the URL and in Enums.ReleasesFilters
	public enum ReleasesFilters
	{
		Clear,
		Tracks,
		EPs,
		NotRemixes,
		Remixes,
		FollowedArtists,
		NotVariousArtists,
		VariousArtists,
		SavedReleases,
		OldReleases,
		NewReleases,
	}

	public enum LocalStorageKey
	{
		LoggedInUser,
		LoginVerifier,
		UserPlaylists,
	}

	public enum MenuButtonsType
	{
		Artists,
		Releases,
		Playlists,
		Date,
	}


	public enum DbStorageTablesSpotify
	{
		SpotifyUpdate,
		SpotifyFilter,

		SpotifyUser,
		SpotifyUserArtist,
		SpotifyUserPlaylist,

		SpotifyPlaylist,
		SpotifyPlaylistTrack,

		SpotifyArtist,
		SpotifyArtistRelease,

		SpotifyRelease,
		SpotifyReleaseTrack,

		SpotifyTrack,
	}

	public enum LoadingType
	{
		Releases,
		Artists,
		Playlists,
		PlaylistTracks,
	}
	public enum LoadingCategory
	{
		GetDb,
		GetApi,
		SaveDb,
	}

	public enum DisplayMobile
	{
		Releases,
		Artists,
		Date,
	}

	public static string GetLocalStorageKey(ServiceType serviceType, LocalStorageKey localStorageKey)
	{
		var prefix = serviceType.ToString();
		var key = localStorageKey.ToString();
		var separator = "_";

		return prefix + separator + key;
	}

	public static ISet<string> GetAllLocalStorageKeys(ServiceType serviceType)
	{
		var keys = EnumUtil.GetValues<LocalStorageKey>();
		var localStorageKeys = new HashSet<string>();
		foreach (var key in keys)
		{
			var localStorageKey = GetLocalStorageKey(serviceType, key);
			localStorageKeys.Add(localStorageKey);
		}
		return localStorageKeys;
	}



	public static string GetLocalStorageKeyReleases(ServiceType serviceType, ReleasesFilters releaseFilters)
	{
		var prefix = serviceType.ToString();
		var prefix2 = "filter";
		var key = releaseFilters.ToString();
		var separator = "_";

		return prefix + separator + prefix2 + separator + key;
	}
}

public static class EnumUtil
{
	public static IEnumerable<T> GetValues<T>()
	{
		return Enum.GetValues(typeof(T)).Cast<T>();
	}
	public static IEnumerable<string> GetNames<T>()
	{
		return GetValues<T>().Select(x => x!.ToString()!);
	}
}