namespace JakubKastner.MusicReleases.Enums;

public static class EnumExtensions
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