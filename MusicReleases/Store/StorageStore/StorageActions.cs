using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

namespace JakubKastner.MusicReleases.Store.StorageStore;

public class StorageActions()
{
	// local storage -> set
	public record StorageActionsSet(SpotifyPlaylistsState PlaylistsState); // persists state
	public record StorageActionsSetSuccess();
	public record StorageActionsSetFailure(string ErrorMessage);

	// local storage -> get
	public record StorageActionsGet();
	public record StorageActionsGetSuccess();
	public record StorageActionsGetFailure(string ErrorMessage);

	// local storage -> clear
	public record StorageActionsClear();
	public record StorageActionsClearSuccess();
	public record StorageActionsClearFailure(string ErrorMessage);
}
