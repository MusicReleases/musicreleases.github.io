namespace JakubKastner.MusicReleases.Base;

public static class Extensions
{
	public static bool IsNullOrEmpty(this string? value)
	{
		return string.IsNullOrEmpty(value?.Trim());
	}
}
