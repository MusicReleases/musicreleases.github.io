namespace JakubKastner.Extensions;

public static class IEnumerableExtensions
{
	public static IEnumerable<T> ApplySearch<T>(this IEnumerable<T> source, string? searchText, params Func<T, string?>[] selectors)
	{
		if (!source.Any())
		{
			return source;
		}

		searchText = searchText.EnsureText();

		if (searchText.IsNullOrEmpty())
		{
			return source;
		}

		// single search
		if (searchText.StartsWith('"') && searchText.EndsWith('"') && searchText.Length > 1)
		{
			var phrase = searchText.Length == 2 ? string.Empty : searchText[1..^1];

			return source.Where
			(
				item => selectors.Any
				(
					sel => sel(item)?.Contains(phrase, StringComparison.OrdinalIgnoreCase) == true
				)
			);
		}

		// multi search
		var words = searchText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

		return source.Where
		(
			item => words.All
			(
				word => selectors.Any
				(
					sel => sel(item)?.Contains(word, StringComparison.OrdinalIgnoreCase) == true
				)
			)
		);
	}

}
