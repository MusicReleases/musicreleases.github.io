using Microsoft.AspNetCore.Components;
using static JakubKastner.MusicReleases.Base.Enums;

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
					_spotifyArtistsController.GetArtists(true);
					break;
				case MenuButtonsType.Releases:
					break;
				case MenuButtonsType.Playlists:
					_spotifyPlaylistsController.GetPlaylists(true);
					break;
				default:
					throw new NotSupportedException(nameof(Type));
			}
		}
	}
}