namespace JakubKastner.Extensions;

public static class TimeSpanExtensions
{
	public static string ToMinuteString(this TimeSpan timeSpan)
	{
		return $"{(int)timeSpan.TotalMinutes:00}m:{timeSpan.Seconds:00}s";
	}

}
