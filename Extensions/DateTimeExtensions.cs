namespace JakubKastner.Extensions;

public static class DateTimeExtensions
{
	public static DateTime ToDateTime(this string value, string? format = null)
	{
		var dateTime = ToDateTimeBase(value, format);
		if (dateTime.HasValue)
		{
			return dateTime.Value;
		}
		throw new NullReferenceException(nameof(DateTime));
	}

	public static DateTime? ToDateTimeNullable(this string? value, string? format = null)
	{
		return ToDateTimeBase(value, format);
	}

	private static DateTime? ToDateTimeBase(string? value, string? format)
	{
		if (value.IsNullOrEmpty())
		{
			return null;
		}

		if (format.IsNullOrEmpty())
		{
			format = value.Length switch
			{
				4 => "yyyy",
				7 => "yyyy-MM",
				10 => "yyyy-MM-dd",
				_ => "yyyy-MM-dd",
			};
		}
		if (DateTime.TryParseExact(value, format, null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
		{
			return dateTime;
		}
		return null;
	}
}
