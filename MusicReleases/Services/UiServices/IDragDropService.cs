using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices
{
	public interface IDragDropService
	{
		object? Payload { get; }
		DragDropType? SourceType { get; }

		void Reset();
		void StartDrag(object item, DragDropType type);
	}
}