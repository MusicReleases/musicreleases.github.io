namespace JakubKastner.MusicReleases.Spotify.Tasks;

public record BackgroundTaskLink
(
	string Text,
	string Title,
	string? UrlApp,
	string UrlWeb,
	Enum Icon
);