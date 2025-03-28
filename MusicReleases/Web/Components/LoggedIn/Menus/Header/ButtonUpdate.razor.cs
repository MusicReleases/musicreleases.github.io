using Microsoft.AspNetCore.Components;
using static JakubKastner.MusicReleases.Base.Enums;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header;

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
		var serviceType = _apiLoginController.GetServiceType();

		if (serviceType == ServiceType.Spotify)
		{
			OnUpdating.InvokeAsync(true);

			switch (Type)
			{
				case MenuButtonsType.Artists:
					// TODO releases type
					_workflowController.StartLoadingArtistsWithReleases(true, ReleaseType.Albums);
					break;
				case MenuButtonsType.Releases:
					// TODO releases type + load only releases without artists
					_workflowController.StartLoadingArtistsWithReleases(true, ReleaseType.Albums);
					break;
				case MenuButtonsType.Playlists:
					_workflowController.StartLoadingPlaylistsWithTracks(true);
					break;
				default:
					throw new NotSupportedException(nameof(Type));
			}
		}
	}
}