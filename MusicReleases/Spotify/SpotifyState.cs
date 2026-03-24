using JakubKastner.SpotifyApi.Objects.Base;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.Spotify;

internal abstract class SpotifyState<TModel> : ISpotifyState<TModel> where TModel : SpotifyIdNameObject
{
	public event Action? OnChange;

	public IReadOnlySet<TModel>? Items => _items?.AsReadOnly();

	public DateTime? LastSync { get; protected set; }


	private readonly ConcurrentDictionary<string, TModel> _lookup = new();

	private SortedSet<TModel>? _items;


	protected void StateChanged() => OnChange?.Invoke();

	public void Set(IReadOnlyCollection<TModel> items, DateTime lastSync)
	{
		_items = new(items);
		LastSync = lastSync;

		_lookup.Clear();
		foreach (var item in items)
		{
			_lookup[item.Id] = item;
		}
		StateChanged();
	}


	public void Add(TModel item)
	{
		_items ??= [];

		_items.Add(item);

		_lookup[item.Id] = item;

		StateChanged();
	}

	public TModel? GetById(string itemId)
	{
		if (itemId.IsNullOrEmpty())
		{
			return null;
		}
		var item = _lookup.TryGetValue(itemId, out var i) ? i : null;
		return item;
	}

	public bool IsInStore(string itemId)
	{
		return GetById(itemId) is not null;
	}
}