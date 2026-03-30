namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal sealed class SpotifyTaskFilterUrlService : IBackgroundTaskFilterUrlService
{
	public string CreateUrlParams(BackgroundTaskFilterType filter, string? searchText)
	{
		var urlParams = new List<string>();

		if (filter != BackgroundTaskFilterType.All)
		{
			var flags = Enum.GetValues<BackgroundTaskFilterType>().Where(f => f != BackgroundTaskFilterType.All && filter.HasFlag(f));

			urlParams.Add("filter=" + string.Join(",", flags));
		}

		if (searchText.IsNotNullOrEmpty())
		{
			urlParams.Add("search=" + searchText);
		}

		if (urlParams.Count == 0)
		{
			return string.Empty;
		}

		return $"?{string.Join("&", urlParams)}";
	}

	public BackgroundTaskFilterType ParseFilterFromUrlParams(string? filterParams)
	{

		if (filterParams.IsNullOrEmpty())
		{
			return BackgroundTaskFilterType.All;
		}

		var filter = (BackgroundTaskFilterType)0;

		var parts = filterParams.Split(',', StringSplitOptions.RemoveEmptyEntries);

		foreach (var p in parts)
		{
			if (Enum.TryParse<BackgroundTaskFilterType>(p, true, out var parsed))
			{
				filter |= parsed;
			}
		}
		return filter;
	}
}
