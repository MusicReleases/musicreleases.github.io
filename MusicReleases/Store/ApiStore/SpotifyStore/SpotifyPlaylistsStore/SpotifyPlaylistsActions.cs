using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

// init
public record SpotifyPlaylistsActionInitialized();

// get
public record SpotifyPlaylistsActionGet(bool ForceUpdate);
public record SpotifyPlaylistsActionGetSuccess();
public record SpotifyPlaylistsActionGetFailure(string ErrorMessage);
// get api
public record SpotifyPlaylistsActionGetApi(SpotifyUserList<SpotifyPlaylist>? Playlists, bool ForceUpdate);
public record SpotifyPlaylistsActionGetApiSuccess();
public record SpotifyPlaylistsActionGetApiFailure(string ErrorMessage);
// get local storage
public record SpotifyPlaylistsActionGetStorage(bool ForceUpdate);
public record SpotifyPlaylistsActionGetStorageSuccess();
public record SpotifyPlaylistsActionGetStorageFailure(string ErrorMessage);

// set
public record SpotifyPlaylistsActionSet(SpotifyUserList<SpotifyPlaylist> Playlists);
// set local storage
public record SpotifyPlaylistsActionSetStorage(SpotifyUserList<SpotifyPlaylist> Playlists);
public record SpotifyPlaylistsActionSetStorageSuccess();
public record SpotifyPlaylistsActionSetStorageFailure(string ErrorMessage);


// TODO persist state
// local storage -> set
public record SpotifyPlaylistsActionSetStorageState(SpotifyPlaylistsState PlaylistsState);
public record SpotifyPlaylistsActionSetStorageStateSuccess();
public record SpotifyPlaylistsActionSetStorageStateFailure(string ErrorMessage);

// local storage -> get
public record SpotifyPlaylistsActionGetStorageState();
public record SpotifyPlaylistsActionGetStorageStateSuccess();
public record SpotifyPlaylistsActionGetStorageStateFailure(string ErrorMessage);

// local storage -> clear
public record SpotifyPlaylistsActionClearStorageState();
public record SpotifyPlaylistsActionClearStorageStateSuccess();
public record SpotifyPlaylistsActionClearStorageStateFailure(string ErrorMessage);