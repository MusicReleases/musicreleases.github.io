namespace JakubKastner.MusicReleases.Enums;


public enum PlaylistVisiblity
{
	Default,
	Public,
	Private,
}

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