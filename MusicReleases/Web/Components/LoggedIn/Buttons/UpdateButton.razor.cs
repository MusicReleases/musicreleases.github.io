using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Buttons;

public partial class UpdateButton : IDisposable
{
	[Inject]
	private ILoadingService LoadingService { get; set; } = default!;

	[Inject]
	private IApiLoginService ApiLoginService { get; set; } = default!;

	[Inject]
	private ISpotifyWorkflowService SpotifyWorkflowService { get; set; } = default!;

	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required UpdateButtonComponent ButtonType { get; set; }

	[Parameter]
	public string? Text { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private string ButtonClass => $"update {ButtonType.ToLowerString()} {Class}";

	private string ButtonTitle => $"Update {ButtonType.ToFriendlyString()}";

	private LucideIcon Icon => IsLoading ? LucideIcon.LoaderCircle : LucideIcon.RefreshCcw;

	private bool IsLoading => ButtonType switch
	{
		UpdateButtonComponent.Artists => LoadingService.IsLoading(BackgroundTaskType.Artists),
		UpdateButtonComponent.Releases => LoadingService.IsLoading(BackgroundTaskType.Releases),
		UpdateButtonComponent.Playlists => LoadingService.IsLoading(BackgroundTaskType.Playlists) || LoadingService.IsLoading(BackgroundTaskType.PlaylistTracks),
		_ => throw new NotImplementedException(),
	};


	protected override void OnInitialized()
	{
		LoadingService.LoadingStateChanged += StateChanged;
	}

	public void Dispose()
	{
		LoadingService.LoadingStateChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void Update()
	{
		var serviceType = ApiLoginService.GetServiceType();

		if (serviceType == ServiceType.Spotify)
		{
			SpotifyWorkflowService.Update(ButtonType, SpotifyReleaseFilterService.Filter.ReleaseGroup);
		}
	}
}