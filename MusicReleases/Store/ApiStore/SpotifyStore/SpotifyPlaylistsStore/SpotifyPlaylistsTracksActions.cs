using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;


// get
public record SpotifyPlaylistsTracksActionGet(bool ForceUpdate, SpotifyUserList<SpotifyPlaylist>? Playlists)
{
	public TaskCompletionSource<bool> CompletionSource { get; } = new TaskCompletionSource<bool>();
}

// get api
public record SpotifyPlaylistsTracksActionGetApi(SpotifyUserList<SpotifyPlaylist>? Playlists, bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistsTracksActionGetApiSuccess();
public record SpotifyPlaylistsTracksActionGetApiFailure(string ErrorMessage);



// TODO persist state
// local storage -> set
public record SpotifyPlaylistsTracksActionSetStorageState(SpotifyPlaylistsTracksState PlaylistsTracksState);
public record SpotifyPlaylistsTracksActionSetStorageStateSuccess();
public record SpotifyPlaylistsTracksActionSetStorageStateFailure(string ErrorMessage);

// local storage -> get
public record SpotifyPlaylistsTracksActionGetStorageState();
public record SpotifyPlaylistsTracksActionGetStorageStateSuccess();
public record SpotifyPlaylistsTracksActionGetStorageStateFailure(string ErrorMessage);

// local storage -> clear
public record SpotifyPlaylistsTracksActionClearStorageState();
public record SpotifyPlaylistsTracksActionClearStorageStateSuccess();
public record SpotifyPlaylistsTracksActionClearStorageStateFailure(string ErrorMessage);