using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
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

	public static string Require(this string? value, [CallerArgumentExpression(nameof(value))] string? paramName = null)
	{
		if (value.IsNullOrEmpty())
		{
			throw new ArgumentException("Value cannot be null or empty.", paramName);
		}

		return value;
	}

	public static string ToKebabCase(this string value)
	{
		if (value.IsNullOrEmpty())
		{
			return string.Empty;
		}

		var sb = new StringBuilder(value.Length + 8);
		for (var i = 0; i < value.Length; i++)
		{
			var c = value[i];
			if (i > 0)
			{
				var prev = value[i - 1];
				if (char.IsUpper(c) || (char.IsDigit(c) && !char.IsDigit(prev)))
				{
					sb.Append('-');
				}
			}
			sb.Append(char.ToLowerInvariant(c));
		}
		return sb.ToString();
	}

	public static string? EnsureText(this string? value)
	{
		return value.IsNullOrEmpty() ? null : value.Trim();
	}
}
