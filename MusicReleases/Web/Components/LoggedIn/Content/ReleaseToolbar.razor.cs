using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseToolbar : IDisposable
{
	[Inject]
	private ILoaderService LoaderService { get; set; } = default!;


	[Parameter(CaptureUnmatchedValues = true)]
	public Dictionary<string, object>? Attributes { get; set; }


	private bool Loading => LoaderService.IsLoading(LoadingType.Artists) || LoaderService.IsLoading(LoadingType.Releases);


	private readonly MenuType _type = MenuType.Releases;


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

	private void AddToPlaylist()
	{

	}

	private void Play()
	{

	}
}
