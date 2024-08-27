using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;


// get
public record SpotifyPlaylistsTracksActionGet(bool ForceUpdate, SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? Playlists)
{
	public TaskCompletionSource<bool> CompletionSource { get; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistsTracksActionGetSuccess()
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistsTracksActionGetFailure(string ErrorMessage)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}

// get api
public record SpotifyPlaylistsTracksActionGetApi(SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? Playlists, bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistsTracksActionGetApiSuccess();
public record SpotifyPlaylistsTracksActionGetApiFailure();



// TODO persist state
// local storage -> set
public record SpotifyPlaylistsTracksActionSetStorageState(SpotifyPlaylistsTracksState PlaylistsTracksState);
public record SpotifyPlaylistsTracksActionSetStorageStateSuccess();
public record SpotifyPlaylistsTracksActionSetStorageStateFailure();

// local storage -> get
public record SpotifyPlaylistsTracksActionGetStorageState();
public record SpotifyPlaylistsTracksActionGetStorageStateSuccess();
public record SpotifyPlaylistsTracksActionGetStorageStateFailure();

// local storage -> clear
public record SpotifyPlaylistsTracksActionClearStorageState();
public record SpotifyPlaylistsTracksActionClearStorageStateSuccess();
public record SpotifyPlaylistsTracksActionClearStorageStateFailure();