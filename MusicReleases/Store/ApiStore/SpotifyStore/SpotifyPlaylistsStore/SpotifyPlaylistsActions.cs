using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

// init
public record SpotifyPlaylistsActionInitialized();

// get playlists
public record SpotifyPlaylistsActionLoad();
public record SpotifyPlaylistsActionLoadSuccess();
public record SpotifyPlaylistsActionLoadFailure(string ErrorMessage);

// set playlists
public record SpotifyPlaylistsActionSet(SpotifyUserList<SpotifyPlaylist> Playlists);

// local storage -> set
public record SpotifyPlaylistsActionStorageSet(SpotifyPlaylistsState PlaylistsState); // persists state
public record SpotifyPlaylistsActionStorageSetSuccess();
public record SpotifyPlaylistsActionStorageSetFailure(string ErrorMessage);

// local storage -> get
public record SpotifyPlaylistsActionStorageGet();
public record SpotifyPlaylistsActionStorageGetSuccess();
public record SpotifyPlaylistsActionStorageGetFailure(string ErrorMessage);

// local storage -> clear
public record SpotifyPlaylistsActionStorageClear();
public record SpotifyPlaylistsActionStorageClearSuccess();
public record SpotifyPlaylistsActionStorageClearFailure(string ErrorMessage);