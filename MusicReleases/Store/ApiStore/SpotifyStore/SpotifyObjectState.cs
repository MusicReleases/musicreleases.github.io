using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore;

public record SpotifyObjectState<T, U> where T : SpotifyIdNameObject where U : SpotifyUserListUpdate
{
	public bool LoadingApi { get; init; } = false;
	public bool LoadingStorage { get; init; } = false;
	public bool Error { get; init; } = false;
	public bool LoadingAny() => LoadingApi || LoadingStorage;
	public SpotifyUserList<T, U> List { get; init; } = new();
}
