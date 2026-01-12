namespace JakubKastner.MusicReleases.Objects.Spotify;

public class SpotifyBackgroundTask
{
	public event Action? OnStateChanged;
	public Guid Id { get; } = Guid.NewGuid();
	public string Name { get; set; } = string.Empty;

	private string _status = string.Empty;
	public string Status
	{
		get => _status;
		set
		{
			if (_status != value)
			{
				_status = value;
				OnStateChanged?.Invoke();
			}
		}
	}

	private double _progress;
	public double Progress
	{
		get => _progress;
		set { _progress = value; OnStateChanged?.Invoke(); }
	}

	private bool _isOverlayVisible = true;
	public bool IsOverlayVisible
	{
		get => _isOverlayVisible;
		set
		{
			if (_isOverlayVisible != value)
			{
				_isOverlayVisible = value;
				OnStateChanged?.Invoke();
			}
		}
	}

	public bool IsRunning { get; set; } = true;
	public bool Failed { get; set; }
}
