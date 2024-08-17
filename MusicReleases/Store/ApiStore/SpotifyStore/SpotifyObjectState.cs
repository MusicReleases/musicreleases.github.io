using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore;

public record SpotifyObjectState<T> where T : SpotifyIdNameObject
{
	public bool LoadingApi { get; init; } = false;
	public bool LoadingStorage { get; init; } = false;
	public bool Error { get; init; } = false;
	public bool LoadingAny() => LoadingApi || LoadingStorage;
	public SpotifyUserList<T> List { get; init; } = new();
}
