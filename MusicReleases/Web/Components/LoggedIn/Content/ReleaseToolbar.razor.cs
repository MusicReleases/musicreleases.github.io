using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseToolbar : IDisposable
{
	[Inject]
	public ILoaderService LoaderService { get; set; } = default!;


	private bool Loading => LoaderService.IsLoading(LoadingType.Releases);


	private const string _buttonClass = "toolbar-releases";


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
