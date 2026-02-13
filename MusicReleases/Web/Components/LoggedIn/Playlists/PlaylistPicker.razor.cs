using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Playlists;

public partial class PlaylistPicker : IDisposable
{
	[Inject]
	private ISpotifyFilterPlaylistService FilterService { get; set; } = default!;


	[Parameter]
	public PlaylistType TypeFilter { get; set; } = PlaylistType.Editable;

	[Parameter]
	public SpotifyRelease? Release { get; set; }

	[Parameter]
	public SpotifyTrack? Track { get; set; }


	protected override void OnParametersSet()
	{
		if (Release is null && Track is null)
		{
			throw new InvalidOperationException($"You must provide either {nameof(Release)} or {nameof(Track)}.");
		}

		if (Release is not null && Track is not null)
		{
			throw new InvalidOperationException($"You must provide only {nameof(Release)} or {nameof(Track)}, not both.");
		}

		FilterService.SetTypeFilter(TypeFilter);
	}

	protected override void OnInitialized()
	{
		FilterService.OnFilterChanged += StateChanged;
	}

	public void Dispose()
	{
		FilterService.OnFilterChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

}
