﻿namespace JakubKastner.MusicReleases.Base;

public static class Extensions
{
	public static bool IsNullOrEmpty(this string? value)
	{
		return string.IsNullOrEmpty(value?.Trim());
	}

	public static DateTime ToDateTime(this string value, string format = "yyyy-MM-dd")
	{
		if (DateTime.TryParseExact(value, format, null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
		{
			return dateTime;
		}
		else
		{
			throw new NullReferenceException(nameof(DateTime));
		}
	}
}
