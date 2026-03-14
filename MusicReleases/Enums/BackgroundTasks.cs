namespace JakubKastner.MusicReleases.Enums;

public enum BackgroundTaskType
{
	ArtistsGet,

	ReleasesGet,
	ReleaseTracksGet,

	PlaylistsGet,
	PlaylistsCreate,

	PlaylistTracksGet,
	PlaylistTracksAdd,
	PlaylistTracksRemove,
}

public enum BackgroundTaskCategory
{
	GetApi,
	SaveApi,
	DeleteApi,
	GetDb,
	SaveDb,
	DeleteDb,
}

public enum BackgroundTaskStatus
{
	Running,
	Canceled,
	Failed,
	Finished,
}