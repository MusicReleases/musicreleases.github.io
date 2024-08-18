using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;

// get artists
public record SpotifyArtistsActionGet(bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyArtistsActionGetSuccess()
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyArtistsActionGetFailure()
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}

// get api
public record SpotifyArtistsActionGetApi(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>? Artists, bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyArtistsActionGetApiSuccess();
public record SpotifyArtistsActionGetApiFailure(string ErrorMessage);

// get local storage
public record SpotifyArtistsActionGetStorage(bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyArtistsActionGetStorageSuccess();
public record SpotifyArtistsActionGetStorageFailure(string ErrorMessage);

// set artists
public record SpotifyArtistsActionSet(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists> Artists)
{
	public ISet<SpotifyArtist> NewArtists { get; set; } = new HashSet<SpotifyArtist>();
}
public record SpotifyArtistsNewActionClear();

// set local storage
public record SpotifyArtistsActionSetStorage(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists> Artists);
public record SpotifyArtistsActionSetStorageSuccess();
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