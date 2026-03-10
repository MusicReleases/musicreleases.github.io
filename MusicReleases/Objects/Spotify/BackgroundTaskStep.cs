using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;

namespace JakubKastner.MusicReleases.Objects.Spotify;

public sealed class BackgroundTaskStep
{
	public required string Name { get; init; }
	public required BackgroundTaskCategory Category { get; init; }

	public BackgroundTaskStatus Status { get; set; } = BackgroundTaskStatus.Queued;
	public DateTimeOffset? StartedAt { get; set; }
	public DateTimeOffset? FinishedAt { get; set; }
	public TimeSpan? Duration => (StartedAt.HasValue && FinishedAt.HasValue) ? FinishedAt - StartedAt : null;

	public int Attempt { get; set; }
	public string? ErrorCode { get; set; }
	public string? ErrorMessage { get; set; }
	public bool Transient { get; set; }

	public BackgroundTaskMetadata Meta { get; } = [];

	public List<BackgroundTaskLink> Links { get; } = [];

	public double SubProgress { get; set; }

	public DateTimeOffset? LastSubProgressAt { get; set; }

	public int SubSeq { get; set; }
}
