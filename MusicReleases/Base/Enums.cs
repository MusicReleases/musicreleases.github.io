﻿namespace JakubKastner.MusicReleases.Base;

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
		UserPlaylists,
		UserPlaylistsState,
	}

	public enum UpdateType
	{
		Artists,
		Releases,
		Playlists,
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
}