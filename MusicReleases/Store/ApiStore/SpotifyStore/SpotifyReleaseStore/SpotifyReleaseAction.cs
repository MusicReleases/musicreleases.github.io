using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleaseStore;

// get releases
public record SpotifyReleaseActionGet(ReleaseType ReleaseType, bool ForceUpdate, ISet<SpotifyArtist> Artists)
{
	public TaskCompletionSource<bool> CompletionSource { get; } = new TaskCompletionSource<bool>();
}
public record SpotifyReleaseActionGetSuccess()
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyReleaseActionGetFailure(string ErrorMessage)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}

// get api
public record SpotifyReleaseActionGetApi(ReleaseType ReleaseType, ISet<SpotifyArtist> Artists, SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? Releases, bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyReleaseActionGetApiSuccess();
public record SpotifyReleaseActionGetApiFailure();

// get local storage
public record SpotifyReleaseActionGetStorage(ReleaseType ReleaseType, ISet<SpotifyArtist> Artists, bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyReleaseActionGetStorageSuccess();
public record SpotifyReleaseActionGetStorageFailure();

// set artists
public record SpotifyReleaseActionSet(SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease> Releases)
{
	public ISet<SpotifyRelease> NewReleases { get; set; } = new HashSet<SpotifyRelease>();
}
public record SpotifyReleaseNewActionClear();

// set local storage
public record SpotifyReleaseActionSetStorage(SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease> Releases);
public record SpotifyReleaseActionSetStorageSuccess();
public record SpotifyReleaseActionSetStorageFailure(string ErrorMessage);


// TODO persist state
// local storage -> set
public record SpotifyReleaseActionSetStorageState(SpotifyReleaseState ReleasesState); // persists state
public record SpotifyReleaseActionSetStorageStateSuccess();
public record SpotifyReleaseActionSetStorageStateFailure();

// local storage -> get
public record SpotifyReleaseActionGetStorageState();
public record SpotifyReleaseActionGetStorageStateSuccess();
public record SpotifyReleaseActionGetStorageStateFailure();

// local storage -> clear
public record SpotifyReleaseActionClearStorageState();
public record SpotifyReleaseActionClearStorageStateSuccess();
public record SpotifyReleaseActionClearStorageStateFailure();