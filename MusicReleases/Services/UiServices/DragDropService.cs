using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public class DragDropService : IDragDropService
{
	public object? Payload { get; private set; }
	public DragDropType? SourceType { get; private set; }

	public void StartDrag(object item, DragDropType type)
	{
		Payload = item;
		SourceType = type;
	}

	public void Reset()
	{
		Payload = null;
		SourceType = null;
	}
}
