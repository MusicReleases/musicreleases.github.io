using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.BackgroundTasks;

public partial class BackgroundTaskFilterButton : IDisposable
{
	[Inject]
	private IBackgroundTaskFilterService SpotifyTaskFilterService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	[Parameter, EditorRequired]
	public required ChipFilterButtonComponent ButtonType { get; set; }

	[Parameter, EditorRequired]
	public required TaskFilter FilterType { get; set; }


	private bool IsFilterActive => SpotifyTaskFilterService.IsActive(FilterType);

	private string ButtonClass => $"filter-tasks {ButtonType.ToLowerString()}";

	private string ButtonTitle => $"{(IsFilterActive ? "Hide" : "Show")} {FilterType.ToFriendlyString()}";

	private string ButtonText => FilterType.ToFriendlyString(true);


	protected override void OnInitialized()
	{
		SpotifyTaskFilterService.OnFilterChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyTaskFilterService.OnFilterChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void ChangeFilter()
	{
		SpotifyTaskFilterService.ToggleFilter(FilterType);
	}
}