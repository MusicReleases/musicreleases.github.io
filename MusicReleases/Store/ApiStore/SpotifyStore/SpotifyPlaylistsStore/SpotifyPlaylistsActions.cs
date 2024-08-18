﻿using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;


// get
public record SpotifyPlaylistsActionGet(bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistsActionGetSuccess()
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistsActionGetFailure()
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}

// get api
public record SpotifyPlaylistsActionGetApi(SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? Playlists, bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistsActionGetApiSuccess();
public record SpotifyPlaylistsActionGetApiFailure(string ErrorMessage);

// get local storage
public record SpotifyPlaylistsActionGetStorage(bool ForceUpdate)
{
	public TaskCompletionSource<bool> CompletionSource { get; init; } = new TaskCompletionSource<bool>();
}
public record SpotifyPlaylistsActionGetStorageSuccess();
public record SpotifyPlaylistsActionGetStorageFailure(string ErrorMessage);

// set
public record SpotifyPlaylistsActionSet(SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists> Playlists);

// set local storage
public record SpotifyPlaylistsActionSetStorage(SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists> Playlists);
public record SpotifyPlaylistsActionSetStorageSuccess();
public record SpotifyPlaylistsActionSetStorageFailure(string ErrorMessage);


// TODO persist state
// local storage -> set
public record SpotifyPlaylistsActionSetStorageState(SpotifyPlaylistsState PlaylistsState);
public record SpotifyPlaylistsActionSetStorageStateSuccess();
public record SpotifyPlaylistsActionSetStorageStateFailure(string ErrorMessage);

// local storage -> get
public record SpotifyPlaylistsActionGetStorageState();
public record SpotifyPlaylistsActionGetStorageStateSuccess();
public record SpotifyPlaylistsActionGetStorageStateFailure(string ErrorMessage);

// local storage -> clear
public record SpotifyPlaylistsActionClearStorageState();
public record SpotifyPlaylistsActionClearStorageStateSuccess();
public record SpotifyPlaylistsActionClearStorageStateFailure(string ErrorMessage);