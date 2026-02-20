using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Shared.Buttons;

public partial class UpdateButton : IDisposable
{
	[Inject]
	private ILoaderService LoaderService { get; set; } = default!;

	[Inject]
	private IApiLoginService ApiLoginService { get; set; } = default!;

	[Inject]
	private ISpotifyWorkflowService SpotifyWorkflowService { get; set; } = default!;

	[Inject]
	private ISpotifyFilterService SpotifyFilterService { get; set; } = default!;


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
		UpdateButtonComponent.Artists => LoaderService.IsLoading(LoadingType.Artists),
		UpdateButtonComponent.Releases => LoaderService.IsLoading(LoadingType.Releases),
		UpdateButtonComponent.Playlists => LoaderService.IsLoading(LoadingType.Playlists) || LoaderService.IsLoading(LoadingType.PlaylistTracks),
		_ => throw new NotImplementedException(),
	};


	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += StateChanged;
	}

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= StateChanged;
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
			if (SpotifyFilterService.Filter is null)
			{
				throw new NullReferenceException(nameof(SpotifyFilterService.Filter));
			}

			SpotifyWorkflowService.Update(ButtonType, SpotifyFilterService.Filter.ReleaseType);
		}
	}
}