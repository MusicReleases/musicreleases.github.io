namespace JakubKastner.MusicReleases.Enums;

public enum LocalStorageKey
{
	LoggedInUser,
	LoginVerifier,
}
public static class EnumStorageExtensions
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
}