using System.Text.RegularExpressions;

namespace JakubKastner.Extensions;

public static class EnumExtensions
{
	public static string ToFriendlyString(this Enum value, bool capitalizeFirstLetter = false)
	{
		var name = value.ToString();
		string friendly;

		// preserve acronyms like EPS or EPs: leave them as-is
		if (Regex.IsMatch(name, @"^[A-Z]{2,}$") || Regex.IsMatch(name, @"^[A-Z][a-z]+s?$"))
		{
			friendly = name;
		}
		else
		{
			// insert spaces before capital letters (camelCase -> separate words) and lowercase everything
			friendly = Regex.Replace(name, "(?<!^)([A-Z])", " $1").ToLower();
		}

		// osptionally capitalize the first letter of the result
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
}