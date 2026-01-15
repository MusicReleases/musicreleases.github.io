namespace JakubKastner.MusicReleases.Enums;

public enum LocalStorageKey
{
	LoggedInUser,
	LoginVerifier,
	UserPlaylists,
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

public enum SpotifyDbUpdateType
{
	User,
	Artists,
	ReleasesAlbums,
	ReleasesTracks,
	ReleasesAppears,
	ReleasesCompilations,
	ReleasesPodcasts,
	ReleasesAlbumsTracks,
	ReleasesTracksTracks,
	ReleasesAppearsTracks,
	ReleasesCompilationsTracks,
	ReleasesPodcastsTracks,
	Playlists,
	PlaylistTracks,
}

public static class EnumDatabaseExtensions
{
	public static string GetLocalStorageKey(ServiceType serviceType, LocalStorageKey localStorageKey)
	{
		var prefix = serviceType.ToString();
		var key = localStorageKey.ToString();
		var separator = "_";

		return prefix + separator + key;
	}

	public static ISet<string> GetAllLocalStorageKeys(ServiceType serviceType)
	{
		var keys = EnumExtensions.GetValues<LocalStorageKey>();
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