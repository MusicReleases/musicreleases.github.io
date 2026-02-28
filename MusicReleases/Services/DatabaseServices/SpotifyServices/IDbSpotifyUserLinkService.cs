namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyUserLinkService
{
	Task<string?> GetReleasesLink(string userId);
	Task<string?> GetTasksLink(string userId);
	Task SetReleasesLink(string userId, string? link);
	Task SetTasksLink(string userId, string? link);
}