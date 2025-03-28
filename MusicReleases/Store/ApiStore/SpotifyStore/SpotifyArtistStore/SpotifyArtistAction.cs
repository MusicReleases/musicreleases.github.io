using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistStore;

// get artists
public record SpotifyArtistActionGet(bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyArtistActionGetSuccess()
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyArtistActionGetFailure(string ErrorMessage)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}

// get api
public record SpotifyArtistActionGetApi(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? Artists, bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyArtistActionGetApiSuccess();
public record SpotifyArtistActionGetApiFailure();

// get local storage
public record SpotifyArtistActionGetStorage(bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyArtistActionGetStorageSuccess();
public record SpotifyArtistActionGetStorageFailure();

// set artists
public record SpotifyArtistActionSet(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain> Artists)
{
	public ISet<SpotifyArtist> NewArtists { get; set; } = new HashSet<SpotifyArtist>();
}
public record SpotifyArtistNewActionClear();

// set local storage
public record SpotifyArtistActionSetStorage(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain> Artists);
public record SpotifyArtistActionSetStorageSuccess();
public record SpotifyArtistActionSetStorageFailure(string ErrorMessage);



// TODO persist state
// local storage -> set
public record SpotifyArtistActionSetStorageState(SpotifyArtistState ArtistsState); // persists state
public record SpotifyArtistActionSetStorageStateSuccess();
public record SpotifyArtistActionSetStorageStateFailure();

// local storage -> get
public record SpotifyArtistActionGetStorageState();
public record SpotifyArtistActionGetStorageStateSuccess();
public record SpotifyArtistActionGetStorageStateFailure();

// local storage -> clear
public record SpotifyArtistActionClearStorageState();
public record SpotifyArtistActionClearStorageStateSuccess();
public record SpotifyArtistActionClearStorageStateFailure();