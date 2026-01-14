using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace JakubKastner.Extensions;

public static class StringExtensions
{
	public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
	{
		return string.IsNullOrWhiteSpace(value);
	}
	public static bool IsNotNullOrEmpty([NotNullWhen(true)] this string? value)
	{
		return !IsNullOrEmpty(value);
	}

	public static string ToKebabCase(this string value)
	{
		if (value.IsNullOrEmpty())
		{
			return "";
		}

		var sb = new StringBuilder(value.Length + 8);
		for (var i = 0; i < value.Length; i++)
		{
			var c = value[i];
			if (char.IsUpper(c) && i > 0)
			{
				sb.Append('-');
			}
			sb.Append(char.ToLowerInvariant(c));
		}
		return sb.ToString();
	}
}
