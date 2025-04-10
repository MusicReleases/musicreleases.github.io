using Microsoft.AspNetCore.Components;
using static JakubKastner.MusicReleases.Base.Enums;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header.Buttons;

public partial class ButtonUpdate
{
	[Parameter, EditorRequired]
	public MenuButtonsType Type { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter, EditorRequired]
	public EventCallback<bool> OnUpdating { get; set; }

	private void Update()
	{
		var serviceType = ApiLoginService.GetServiceType();

		if (serviceType == ServiceType.Spotify)
		{
			OnUpdating.InvokeAsync(true);

			switch (Type)
			{
				case MenuButtonsType.Artists:
					// TODO releases type
					SpotifyWorkflowController.StartLoadingArtistsWithReleases(true, ReleaseType.Albums);
					break;
				case MenuButtonsType.Releases:
					// TODO releases type + load only releases without artists
					SpotifyWorkflowController.StartLoadingArtistsWithReleases(true, ReleaseType.Albums);
					break;
				case MenuButtonsType.Playlists:
					SpotifyWorkflowController.StartLoadingPlaylistsWithTracks(true);
					break;
				default:
					throw new NotSupportedException(nameof(Type));
			}
		}
	}
}