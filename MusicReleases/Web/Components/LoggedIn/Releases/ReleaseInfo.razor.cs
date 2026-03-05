using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseInfo : IDisposable
{
	[Inject]
	private IDragDropService DragDropService { get; set; } = default!;

	[Inject]
	private ISpotifyFilterUrlService SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;

	[Inject]
	private ISettingsService SettingsService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required SpotifyRelease SpotifyRelease { get; set; }


	private string ReleaseDateDay => $"{SpotifyRelease.ReleaseDate:dd}.";

	private string ReleaseDateMonth => $"{SpotifyRelease.ReleaseDate:MM}.";

	private string ReleaseDateYear => $"{SpotifyRelease.ReleaseDate:yyyy}";

	private LucideIcon ReleaseTypeIcon => EnumIconsExtensions.GetIconForReleaseType(SpotifyRelease.ReleaseType);

	private string ReleaseTypeIconTitle => $"{SpotifyRelease.ReleaseType} release";


	private const string _buttonClass = "release-info";


	private void OnDragStart()
	{
		DragDropService.StartDrag(SpotifyRelease, DragDropType.Release);
	}

	private async Task FilterMonth()
	{
		var url = await SpotifyFilterUrlService.GetFilterUrl(SpotifyRelease.ReleaseDate.Year, SpotifyRelease.ReleaseDate.Month);
		NavManager.NavigateTo(url);
	}

	private async Task FilterYear()
	{
		var url = await SpotifyFilterUrlService.GetFilterUrl(SpotifyRelease.ReleaseDate.Year);
		NavManager.NavigateTo(url);
	}
	protected override void OnInitialized()
	{
		SettingsService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		SettingsService.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
