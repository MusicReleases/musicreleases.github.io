using System.Text.RegularExpressions;

namespace JakubKastner.Extensions;

public static class EnumExtensions
{
	public static string ToFriendlyString(this Enum value, bool capitalizeFirstLetter = false)
	{
		var name = value.ToString();

		if (name.IsNullOrEmpty())
		{
			return string.Empty;
		}

		string friendly;

		// keep acronyms like EPS or EPs: leave them as-is
		if (Regex.IsMatch(name, @"^[A-Z0-9]{2,}s?$"))
		{
			friendly = name;
		}

		else
		{
			// replace _ or -
			var normalized = Regex.Replace(name, @"[_\-]+", " ");

			// insert spaces between words
			normalized = Regex.Replace(normalized, @"(?<=[a-z0-9])([A-Z])", " $1");

			// acronyms and words
			normalized = Regex.Replace(normalized, @"(?<=[A-Z])([A-Z][a-z])", " $1");

			friendly = normalized.ToLowerInvariant();

			// repalce multi spaces
			friendly = Regex.Replace(friendly, @"\s{2,}", " ").Trim();
		}

		// optionally capitalize the first letter of the result
		if (capitalizeFirstLetter && friendly.Length > 0)
		{
			friendly = char.ToUpper(friendly[0]) + friendly.Substring(1);
		}

		return friendly;
	}

	public static IEnumerable<T> GetValues<T>()
	{
		return Enum.GetValues(typeof(T)).Cast<T>();
	}
	public static IEnumerable<string> GetNames<T>()
	{
		return GetValues<T>().Select(x => x!.ToString()!);
	}
	public static bool HasAnyFlag<T>(this T value, params T[] flags) where T : Enum
	{
		foreach (var flag in flags)
		{
			if (value.HasFlag(flag))
			{
				return true;
			}
		}
		return false;
	}
}