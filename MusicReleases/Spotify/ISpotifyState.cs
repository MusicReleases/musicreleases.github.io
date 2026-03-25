using JakubKastner.SpotifyApi.Base.Objects;

namespace JakubKastner.MusicReleases.Spotify;

internal interface ISpotifyState<TModel> where TModel : SpotifyIdNameObject
{
	IReadOnlySet<TModel>? Items { get; }
	DateTime? LastSync { get; }

	event Action? OnChange;

	void Add(TModel item);
	TModel? GetById(string itemId);
	bool IsInStore(string itemId);
	void Merge(IEnumerable<TModel> items, DateTime lastSync);
	void Set(IReadOnlyCollection<TModel> items, DateTime lastSync);
}