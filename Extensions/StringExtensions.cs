using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.Extensions;

public static class StringExtensions
{
	public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
	{
		return string.IsNullOrEmpty(value?.Trim());
	}
	public static bool IsNotNullOrEmpty([NotNullWhen(true)] this string? value)
	{
		return !IsNullOrEmpty(value);
	}
}
