using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

// get releases
public record SpotifyReleasesActionGet(ReleaseType ReleaseType, bool ForceUpdate, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? Artists)
{
	public TaskCompletionSource<bool> CompletionSource { get; } = new TaskCompletionSource<bool>();
}
public record SpotifyReleasesActionGetSuccess()
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyReleasesActionGetFailure(string ErrorMessage)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}

// get api
public record SpotifyReleasesActionGetApi(ReleaseType ReleaseType, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? Artists, SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? Releases, bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyReleasesActionGetApiSuccess();
public record SpotifyReleasesActionGetApiFailure();

// get local storage
public record SpotifyReleasesActionGetStorage(ReleaseType ReleaseType, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain> Artists, bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyReleasesActionGetStorageSuccess();
public record SpotifyReleasesActionGetStorageFailure();

// set artists
public record SpotifyReleasesActionSet(SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease> Releases)
{
	public ISet<SpotifyRelease> NewReleases { get; set; } = new HashSet<SpotifyRelease>();
}
public record SpotifyReleasesNewActionClear();

// set local storage
public record SpotifyReleasesActionSetStorage(SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease> Releases);
public record SpotifyReleasesActionSetStorageSuccess();
public record SpotifyReleasesActionSetStorageFailure(string ErrorMessage);


// TODO persist state
// local storage -> set
public record SpotifyReleasesActionSetStorageState(SpotifyReleasesState ReleasesState); // persists state
public record SpotifyReleasesActionSetStorageStateSuccess();
public record SpotifyReleasesActionSetStorageStateFailure();

// local storage -> get
public record SpotifyReleasesActionGetStorageState();
public record SpotifyReleasesActionGetStorageStateSuccess();
public record SpotifyReleasesActionGetStorageStateFailure();

// local storage -> clear
public record SpotifyReleasesActionClearStorageState();
public record SpotifyReleasesActionClearStorageStateSuccess();
public record SpotifyReleasesActionClearStorageStateFailure();