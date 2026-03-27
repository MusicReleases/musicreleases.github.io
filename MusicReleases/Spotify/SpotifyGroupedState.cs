using JakubKastner.SpotifyApi.Base.Objects;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.Spotify;

internal abstract class SpotifyGroupedState<TModel, TGroupKey> : ISpotifyGroupedState<TModel, TGroupKey> where TModel : SpotifyIdNameObject
	where TGroupKey : notnull
{
	public event Action? OnChange;

	public ConcurrentDictionary<TGroupKey, IReadOnlySet<TModel>> Items { get; } = new();
	public ConcurrentDictionary<TGroupKey, DateTime> LastSync { get; } = new();

	public void Set(TGroupKey group, IEnumerable<TModel> items, DateTime lastSync)
	{
		Items[group] = new SortedSet<TModel>(items).AsReadOnly();
		LastSync[group] = lastSync;
		OnChange?.Invoke();
	}


	public void Merge(TGroupKey group, IEnumerable<TModel> newItems, DateTime lastSync)
	{
		Items.AddOrUpdate(
			group,
			_ => new SortedSet<TModel>(newItems).AsReadOnly(),
			(_, existing) =>
			{
				var merged = new SortedSet<TModel>(existing);
				merged.UnionWith(newItems);
				return merged.AsReadOnly();
			});

		LastSync[group] = lastSync;
		//OnChange?.Invoke();
	}
}
