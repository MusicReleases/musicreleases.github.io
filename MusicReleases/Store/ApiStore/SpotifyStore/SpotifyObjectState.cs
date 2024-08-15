using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore;

public record SpotifyObjectState<T> where T : SpotifyIdObject
{
	public bool Initialized { get; init; }
	public bool LoadingApi { get; init; }
	public bool LoadingStorage { get; init; }
	public bool Error { get; init; }
	public bool LoadingAny() => LoadingApi || LoadingStorage;
	public SpotifyUserList<T>? List { get; init; }
}
