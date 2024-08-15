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
public record SpotifyPlaylistsActionStorageStateSet(SpotifyPlaylistsState PlaylistsState); // persists state
public record SpotifyPlaylistsActionStorageSetStateSuccess();
public record SpotifyPlaylistsActionStorageSetStateFailure(string ErrorMessage);

// local storage -> get
public record SpotifyPlaylistsActionStorageGetState();
public record SpotifyPlaylistsActionStorageGetStateSuccess();
public record SpotifyPlaylistsActionStorageGetStateFailure(string ErrorMessage);

// local storage -> clear
public record SpotifyPlaylistsActionStorageStateClear();
public record SpotifyPlaylistsActionStorageClearStateSuccess();
public record SpotifyPlaylistsActionStorageClearStateFailure(string ErrorMessage);