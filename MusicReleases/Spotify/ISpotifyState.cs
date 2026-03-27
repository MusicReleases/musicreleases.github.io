using JakubKastner.SpotifyApi.Base.Objects;

namespace JakubKastner.MusicReleases.Spotify;

internal interface ISpotifyState<TModel> where TModel : SpotifyIdNameObject
{
	IReadOnlySet<TModel>? Items { get; }
	DateTime? LastSync { get; }

	event Action? OnChange;

	void Add(TModel item);
	void AddRange(IEnumerable<TModel> items, DateTime lastSync, bool notify);
	TModel? GetById(string itemId);
	bool IsInStore(string itemId);
	void MergeDelta(IEnumerable<TModel> items, DateTime lastSync);
	void ReconcileSnapshot(IReadOnlyCollection<TModel> snapshot, DateTime lastSync);
	void Set(IReadOnlyCollection<TModel> items, DateTime lastSync);
}