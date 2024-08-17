using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

// init
public record SpotifyReleasesActionInitialized();
public record SpotifyReleasesActionLoadSuccess();
public record SpotifyReleasesActionLoadFailure(string ErrorMessage);


// get releases
public record SpotifyReleasesActionGet(ReleaseType ReleaseType, bool ForceUpdate);
public record SpotifyReleasesActionGetSuccess();

// get api
public record SpotifyReleasesActionGetApi(ReleaseType ReleaseType, SpotifyUserListReleases<SpotifyRelease>? Releases, bool ForceUpdate);
public record SpotifyReleasesActionGetApiSuccess();
public record SpotifyReleasesActionGetApiFailure(string ErrorMessage);

// get local storage
public record SpotifyReleasesActionGetStorage(ReleaseType ReleaseType, bool ForceUpdate);
public record SpotifyReleasesActionGetStorageSuccess();
public record SpotifyReleasesActionGetStorageFailure(string ErrorMessage);

// set releases
public record SpotifyReleasesActionSet(SpotifyUserListReleases<SpotifyRelease> Releases);

// set local storage
public record SpotifyReleasesActionSetStorage(SpotifyUserListReleases<SpotifyRelease> Releases);
public record SpotifyReleasesActionSetStorageSuccess();
public record SpotifyReleasesActionSetStorageFailure(string ErrorMessage);



// TODO persist state
// local storage -> set
public record SpotifyReleasesActionSetStorageState(SpotifyReleasesState ArtistsState); // persists state
public record SpotifyReleasesActionSetStorageStateSuccess();
public record SpotifyReleasesActionSetStorageStateFailure(string ErrorMessage);

// local storage -> get
public record SpotifyReleasesActionGetStorageState();
public record SpotifyReleasesActionGetStorageStateSuccess();
public record SpotifyReleasesActionGetStorageStateFailure(string ErrorMessage);

// local storage -> clear
public record SpotifyReleasesActionClearStorageState();
public record SpotifyReleasesActionClearStorageStateSuccess();
public record SpotifyReleasesActionClearStorageStateFailure(string ErrorMessage);