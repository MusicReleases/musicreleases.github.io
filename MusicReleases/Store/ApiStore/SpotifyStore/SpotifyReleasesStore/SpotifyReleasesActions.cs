using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

// get releases
public record SpotifyReleasesActionGet(ReleaseType ReleaseType, bool ForceUpdate, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>? Artists)
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
public record SpotifyReleasesActionGetApi(ReleaseType ReleaseType, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>? Artists, bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyReleasesActionGetApiSuccess();
public record SpotifyReleasesActionGetApiFailure(string ErrorMessage)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}

// get local storage
public record SpotifyReleasesActionGetStorage(ReleaseType ReleaseType, bool ForceUpdate);
public record SpotifyReleasesActionGetStorageSuccess();
public record SpotifyReleasesActionGetStorageFailure();



// TODO persist state
// local storage -> set
public record SpotifyReleasesActionSetStorageState(SpotifyReleasesState ArtistsState); // persists state
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