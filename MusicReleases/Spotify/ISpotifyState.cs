using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Spotify;

internal interface ISpotifyState<TModel> where TModel : SpotifyIdNameObject
{
	IReadOnlySet<TModel>? Items { get; }
	DateTime? LastSync { get; }

	event Action? OnChange;

	void Add(TModel item);
	TModel? GetById(string itemId);
	bool IsInStore(string itemId);
	void Set(IReadOnlyCollection<TModel> items, DateTime lastSync);
}