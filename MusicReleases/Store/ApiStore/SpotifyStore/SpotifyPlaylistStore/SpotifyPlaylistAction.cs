using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistStore;


// get
public record SpotifyPlaylistActionGet(bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistActionGetSuccess()
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistActionGetFailure(string ErrorMessage)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}

// get api
public record SpotifyPlaylistActionGetApi(SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? Playlists, bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistActionGetApiSuccess();
public record SpotifyPlaylistActionGetApiFailure();

// get local storage
public record SpotifyPlaylistActionGetStorage(bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistActionGetStorageSuccess();
public record SpotifyPlaylistActionGetStorageFailure();

// set
public record SpotifyPlaylistActionSet(SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists> Playlists);

// set local storage
public record SpotifyPlaylistActionSetStorage(SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists> Playlists);
public record SpotifyPlaylistActionSetStorageSuccess();
public record SpotifyPlaylistActionSetStorageFailure(string ErrorMessage);


// TODO persist state
// local storage -> set
public record SpotifyPlaylistActionSetStorageState(SpotifyPlaylistState PlaylistsState);
public record SpotifyPlaylistActionSetStorageStateSuccess();
public record SpotifyPlaylistActionSetStorageStateFailure();

// local storage -> get
public record SpotifyPlaylistActionGetStorageState();
public record SpotifyPlaylistActionGetStorageStateSuccess();
public record SpotifyPlaylistActionGetStorageStateFailure();

// local storage -> clear
public record SpotifyPlaylistActionClearStorageState();
public record SpotifyPlaylistActionClearStorageStateSuccess();
public record SpotifyPlaylistActionClearStorageStateFailure();