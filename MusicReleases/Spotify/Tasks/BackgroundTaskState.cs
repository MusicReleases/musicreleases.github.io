namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal sealed class BackgroundTaskState : IBackgroundTaskState
{
	private readonly IBackgroundTaskManagerService _manager;
	private readonly IBackgroundTaskFilterService _filter;

	public event Action? OnChange;

	public IReadOnlyList<BackgroundTask> Tasks => _filter.Filtered;

	public BackgroundTaskState(IBackgroundTaskManagerService manager, IBackgroundTaskFilterService filter)
	{
		_manager = manager;
		_filter = filter;

		_manager.OnUiRelevantChange += OnManagerUiChanged;
		_filter.OnFilterChanged += OnFilterChanged;

		_filter.SetSource(_manager.AllTasks);
	}

	public void Dispose()
	{
		_manager.OnUiRelevantChange -= OnManagerUiChanged;
		_filter.OnFilterChanged -= OnFilterChanged;
	}

	private void OnManagerUiChanged()
	{
		Console.WriteLine("manager changed!!!!!");
		_filter.SetSource(_manager.AllTasks);
		OnChange?.Invoke();
	}

	private void OnFilterChanged()
	{
		Console.WriteLine("filter changed!!!!!");
		OnChange?.Invoke();
	}
}