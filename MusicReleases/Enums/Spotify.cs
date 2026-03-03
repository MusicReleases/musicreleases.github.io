namespace JakubKastner.MusicReleases.Enums;

// this names must be same as in the URL and in Enums.ReleasesFilters
public enum ReleasesFilters
{
	Clear,
	Tracks,
	EPs,
	NotRemixes,
	Remixes,
	FollowedArtists,
	NotVariousArtists,
	VariousArtists,
	SavedReleases,
	OldReleases,
	NewReleases,
}

public enum FilterType
{
	Any,
	Artist,
	Date,
	Advanced,
}

[Flags]
public enum TaskFilter
{
	Visible = 1 << 0, // 1
	Hidden = 1 << 1, // 2
	Running = 1 << 2, // 4
	Finished = 1 << 3, // 8
	Failed = 1 << 4, // 16
	Succeeded = 1 << 5, // 32

	All = Visible | Hidden | Running | Finished | Failed | Succeeded,
}