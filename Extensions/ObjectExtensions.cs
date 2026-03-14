using System.Runtime.CompilerServices;

namespace JakubKastner.Extensions;

public static class ObjectExtensions
{
	public static string ToLowerString(this object obj)
	{
		return obj.ToString()!.ToLower();
	}

	public static T Require<T>(this T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : class
	{
		if (value is null)
		{
			throw new ArgumentNullException(paramName);
		}

		return value;
	}
	public static T Require<T>(this T? value, [CallerArgumentExpression(nameof(value))] string? paramName = null) where T : struct
	{
		if (!value.HasValue)
		{
			throw new ArgumentNullException(paramName);
		}

		return value.Value;
	}
}