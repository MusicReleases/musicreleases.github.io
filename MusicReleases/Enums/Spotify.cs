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
public enum ReleaseAdvancedFilter
{
	Albums = 1 << 0, // 1
	Tracks = 1 << 1, // 2
	EPs = 1 << 2, // 4
	Compilations = 1 << 3, // 8
	NotRemixes = 1 << 4, // 16
	Remixes = 1 << 5, // 32
	FollowedArtists = 1 << 6, // 64
	SavedReleases = 1 << 7, // 128
	NotVariousArtists = 1 << 8, // 256
	VariousArtists = 1 << 9, // 512
	OldReleases = 1 << 10, // 1024
	NewReleases = 1 << 11,

	All = Albums | Tracks | EPs | Compilations | NotRemixes | Remixes | FollowedArtists | SavedReleases | NotVariousArtists | VariousArtists | OldReleases | NewReleases,
}

[Flags]
public enum TaskFilter
{
	Running = 1 << 0, // 1
	Canceled = 1 << 1, // 2
	Failed = 1 << 2, // 4
	Finished = 1 << 3, // 8

	All = Running | Canceled | Failed | Finished,
}