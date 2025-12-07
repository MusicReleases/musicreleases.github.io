namespace JakubKastner.MusicReleases.Objects.Spotify;

public class SpotifyBackgroundTask
{
	public Guid Id { get; } = Guid.NewGuid();
	public string Name { get; set; } = string.Empty;
	public string Status { get; set; } = "Pending";
	public double? Progress { get; set; } // 0.0 to 1.0
	public bool IsRunning { get; set; } = true;
	public bool Failed { get; set; }
}
