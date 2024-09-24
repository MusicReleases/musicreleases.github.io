using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.Extensions;

public static class DateTimeExtensions
{
	public static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
	{
		return string.IsNullOrEmpty(value?.Trim());
	}
}
