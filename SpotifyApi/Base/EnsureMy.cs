namespace JakubKastner.SpotifyApi.Base;

public static class EnsureMe
{
	//
	// Summary:
	//     Checks an argument to ensure it isn't null.
	//
	// Parameters:
	//   value:
	//     The argument value to check
	//
	//   name:
	//     The name of the argument
	public static void ArgumentNotNull(object value, string name)
	{
		if (value != null)
		{
			return;
		}

		throw new ArgumentNullException(name);
	}

	//
	// Summary:
	//     Checks an argument to ensure it isn't null or an empty string
	//
	// Parameters:
	//   value:
	//     The argument value to check
	//
	//   name:
	//     The name of the argument
	public static void ArgumentNotNullOrEmptyString(string value, string name)
	{
		if (!string.IsNullOrEmpty(value))
		{
			return;
		}

		throw new ArgumentException("String is empty or null", name);
	}

	public static void ArgumentNotNullOrEmptyList<T>(IEnumerable<T> value, string name)
	{
		if (value != null && value.Any())
		{
			return;
		}

		throw new ArgumentException("List is empty or null", name);
	}
}