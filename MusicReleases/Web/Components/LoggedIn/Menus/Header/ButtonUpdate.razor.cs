using Microsoft.AspNetCore.Components;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header;

public partial class ButtonUpdate
{
	[Parameter, EditorRequired]
	public UpdateType Type { get; set; }

	private void Update()
	{
		var serviceType = _apiLoginController.GetServiceType();

		if (serviceType == ServiceType.Spotify)
		{
			switch (Type)
			{
				case UpdateType.Artists:
					break;
				case UpdateType.Releases:
					break;
				case UpdateType.Playlists:
					_spotifyPlaylistsController.GetPlaylists(true);
					break;
				default:
					throw new NotSupportedException(nameof(Type));
			}
		}
	}
}