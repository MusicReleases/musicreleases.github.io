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
	NotRemixes = 1 << 3, // 8
	Remixes = 1 << 4, // 16
	FollowedArtists = 1 << 5, // 32
	SavedReleases = 1 << 6, // 64
	NotVariousArtists = 1 << 7, // 128
	VariousArtists = 1 << 8, // 256
	OldReleases = 1 << 9, // 512
	NewReleases = 1 << 10, // 1024

	All = Albums | Tracks | EPs | NotRemixes | Remixes | FollowedArtists | SavedReleases | NotVariousArtists | VariousArtists | OldReleases | NewReleases,
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