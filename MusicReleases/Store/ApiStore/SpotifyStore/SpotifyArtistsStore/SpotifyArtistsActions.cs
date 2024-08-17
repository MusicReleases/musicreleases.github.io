using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;

// init
public record SpotifyArtistsActionInitialized();
public record SpotifyArtistsActionLoadSuccess();
public record SpotifyArtistsActionLoadFailure(string ErrorMessage);


// get artists
public record SpotifyArtistsActionGet(bool ForceUpdate);
public record SpotifyArtistsActionGetSuccess(bool ForceUpdate);

// get api
public record SpotifyArtistsActionGetApi(SpotifyUserList<SpotifyArtist>? Artists, bool ForceUpdate);
public record SpotifyArtistsActionGetApiSuccess();
public record SpotifyArtistsActionGetApiFailure(string ErrorMessage);

// get local storage
public record SpotifyArtistsActionGetStorage(bool ForceUpdate);
public record SpotifyArtistsActionGetStorageSuccess();
public record SpotifyArtistsActionGetStorageFailure(string ErrorMessage);

// set artists
public record SpotifyArtistsActionSet(SpotifyUserList<SpotifyArtist> Artists, ISet<SpotifyArtist> NewArtists);
public record SpotifyArtistsNewActionClear();

// set local storage
public record SpotifyArtistsActionSetStorage(SpotifyUserList<SpotifyArtist> Artists, bool ForceUpdate);
public record SpotifyArtistsActionSetStorageSuccess(bool ForceUpdate);
public record SpotifyArtistsActionSetStorageFailure(string ErrorMessage);



// TODO persist state
// local storage -> set
public record SpotifyArtistsActionSetStorageState(SpotifyArtistsState ArtistsState); // persists state
public record SpotifyArtistsActionSetStorageStateSuccess();
public record SpotifyArtistsActionSetStorageStateFailure(string ErrorMessage);

// local storage -> get
public record SpotifyArtistsActionGetStorageState();
public record SpotifyArtistsActionGetStorageStateSuccess();
public record SpotifyArtistsActionGetStorageStateFailure(string ErrorMessage);

// local storage -> clear
public record SpotifyArtistsActionClearStorageState();
public record SpotifyArtistsActionClearStorageStateSuccess();
public record SpotifyArtistsActionClearStorageStateFailure(string ErrorMessage);