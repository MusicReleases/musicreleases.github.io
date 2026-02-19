namespace JakubKastner.Extensions;

public static class ObjectExtensions
{
	public static string ToLowerString(this object obj)
	{
		return obj.ToString()!.ToLower();
	}
}
