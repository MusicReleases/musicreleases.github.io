using System.Globalization;

namespace JakubKastner.Extensions;

public static class DateTimeOffsetExtensions
{
	public static string ToCurrentCultureString(this DateTimeOffset dateTimeOffset)
	{
		return dateTimeOffset.ToLocalTime().ToString("g", CultureInfo.CurrentCulture);
	}

}
