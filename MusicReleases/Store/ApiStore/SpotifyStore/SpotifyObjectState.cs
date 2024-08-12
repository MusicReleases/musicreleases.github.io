using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore;

public record SpotifyObjectState<T> where T : SpotifyIdObject
{
	public bool Initialized { get; init; }
	public bool Loading { get; init; }
	public SpotifyUserList<T>? List { get; init; }
}
