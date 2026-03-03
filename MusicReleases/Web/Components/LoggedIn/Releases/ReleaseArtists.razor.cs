using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseArtists : IDisposable
{
	[Inject]
	private ISettingsService SettingsService { get; set; } = default!;

	[Parameter, EditorRequired]
	public required HashSet<SpotifyArtist> Artists { get; set; }

	[Parameter]
	public string? ButtonClass { get; set; }

	[Parameter]
	public string? SpanClass { get; set; }


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
