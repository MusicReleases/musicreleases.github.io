using JakubKastner.MusicReleases.Enums;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Objects.Spotify;

public class BackgroundTask
{
	public event Action? OnStateChanged;

	public Guid Id { get; } = Guid.NewGuid();

	public required string Name { get; init; }

	public required BackgroundTaskType Type { get; init; }


	private string _status = string.Empty;

	public string Status
	{
		get => _status;
		set
		{
			if (_status != value)
			{
				_status = value;
				NotifyChange();
			}
		}
	}

	private double _progress;
	public double Progress
	{
		get => _progress;
		set
		{
			_progress = value;
			NotifyChange();
		}
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
				NotifyChange();
			}
		}
	}

	public bool IsRunning { get; set; } = true;

	public bool Failed { get; set; }


	public int CurrentStepIndex { get; set; }

	public List<BackgroundTaskStep> Steps { get; } = [];
	public List<BackgroundTaskLink> Links { get; } = [];



	[SetsRequiredMembers]
	public BackgroundTask(string name, BackgroundTaskType type, string status = "Starting...")
	{
		Name = name;
		Type = type;
		Status = status;
	}

	public void NotifyChange()
	{
		OnStateChanged?.Invoke();
	}


	public void RecalculateProgress()
	{
		if (Steps.Count == 0)
		{
			Progress = 0;
			return;
		}

		var idx = CurrentStepIndex;
		var total = Steps.Count;

		var basePart = Math.Clamp((double)idx / total, 0, 1);

		var step = Steps[idx];
		var sub = Math.Clamp(step.SubProgress, 0, 1) / total;

		Progress = Math.Clamp(basePart + sub, 0, 1);
	}

}
