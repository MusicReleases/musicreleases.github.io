namespace JakubKastner.MusicReleases.Objects.Spotify;

public sealed class BackgroundTaskLink
{
	public required string Label { get; init; }
	public required string UrlWeb { get; init; }
	public required string UrlApp { get; init; }
	public string? Rel { get; init; }
}
