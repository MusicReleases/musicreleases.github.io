namespace JakubKastner.MusicReleases.Enums;

public enum BackgroundTaskType
{
	Releases,
	Artists,
	Playlists,
	PlaylistTracks,
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