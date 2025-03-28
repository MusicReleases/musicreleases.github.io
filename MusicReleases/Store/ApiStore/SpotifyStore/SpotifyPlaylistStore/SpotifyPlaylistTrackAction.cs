using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;


// get
public record SpotifyPlaylistTrackActionGet(bool ForceUpdate, SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? Playlists)
{
	public TaskCompletionSource<bool> CompletionSource { get; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistTrackActionGetSuccess()
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistTrackActionGetFailure(string ErrorMessage)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}

// get api
public record SpotifyPlaylistTrackActionGetApi(SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? Playlists, bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistTrackActionGetApiSuccess();
public record SpotifyPlaylistTrackActionGetApiFailure();



// TODO persist state
// local storage -> set
public record SpotifyPlaylistTrackActionSetStorageState(SpotifyPlaylistTrackState PlaylistTrackState);
public record SpotifyPlaylistTrackActionSetStorageStateSuccess();
public record SpotifyPlaylistTrackActionSetStorageStateFailure();

// local storage -> get
public record SpotifyPlaylistTrackActionGetStorageState();
public record SpotifyPlaylistTrackActionGetStorageStateSuccess();
public record SpotifyPlaylistTrackActionGetStorageStateFailure();

// local storage -> clear
public record SpotifyPlaylistTrackActionClearStorageState();
public record SpotifyPlaylistTrackActionClearStorageStateSuccess();
public record SpotifyPlaylistTrackActionClearStorageStateFailure();