using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Artists;

// init
public record SpotifyArtistsActionInitialized();

// get artists
public record SpotifyArtistsActionLoad();
public record SpotifyArtistsActionLoadSuccess();
public record SpotifyArtistsActionLoadFailure(string ErrorMessage);

// set artists
public record SpotifyArtistsActionSet(SortedSet<Artist> Artists);

// local storage -> set
public record SpotifyArtistsActionStorageSet(SpotifyArtistsState ArtistsState); // persists state
public record SpotifyArtistsActionStorageSetSuccess();
public record SpotifyArtistsActionStorageSetFailure(string ErrorMessage);

// local storage -> get
public record SpotifyArtistsActionStorageGet();
public record SpotifyArtistsActionStorageGetSuccess();
public record SpotifyArtistsActionStorageGetFailure(string ErrorMessage);

// local storage -> clear
public record SpotifyArtistsActionStorageClear();
public record SpotifyArtistsActionStorageClearSuccess();
public record SpotifyArtistsActionStorageClearFailure(string ErrorMessage);