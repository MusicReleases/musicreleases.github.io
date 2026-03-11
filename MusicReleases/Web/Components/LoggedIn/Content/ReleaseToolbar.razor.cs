using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseToolbar : IDisposable
{
	[Inject]
	public ILoadingService LoadingService { get; set; } = default!;


	private bool Loading => LoadingService.IsLoading(BackgroundTaskType.Releases);


	private const string _buttonClass = "toolbar-releases";


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

	private void AddToPlaylist()
	{

	}

	private void Play()
	{

	}
}
