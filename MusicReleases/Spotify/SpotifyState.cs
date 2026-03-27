using JakubKastner.SpotifyApi.Base.Objects;
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
		_items = [.. items];
		LastSync = lastSync;

		RecalculateLookup();
		StateChanged();
	}

	private void RecalculateLookup()
	{
		_lookup.Clear();
		if (_items is null)
		{
			return;
		}
		foreach (var item in _items)
		{
			_lookup[item.Id] = item;
		}
	}

	public void Add(TModel item)
	{
		_items ??= [];

		// replace-by-id semantics
		if (_lookup.TryGetValue(item.Id, out var old))
		{
			var merged = PreserveUserFlags(old, item);
			_items.Remove(old);
			_items.Add(merged);
			_lookup[item.Id] = merged;
		}
		else
		{
			_items.Add(item);
			_lookup[item.Id] = item;
		}

		StateChanged();
	}

	public void AddRange(IEnumerable<TModel> items, DateTime lastSync, bool notify)
	{
		_items ??= [];

		foreach (var item in items)
		{
			if (_lookup.TryGetValue(item.Id, out var old))
			{
				var merged = PreserveUserFlags(old, item);
				_items.Remove(old);
				_items.Add(merged);
				_lookup[item.Id] = merged;
			}
			else
			{
				_items.Add(item);
				_lookup[item.Id] = item;
			}
		}

		LastSync = lastSync;

		if (notify)
		{
			StateChanged();
		}
	}

	public void MergeDelta(IEnumerable<TModel> items, DateTime lastSync)
	{
		AddRange(items, lastSync, true);
	}

	public void ReconcileSnapshot(IReadOnlyCollection<TModel> snapshot, DateTime lastSync)
	{
		_items ??= [];

		// keep old instances for user flags (e.g. SpotifyArtist.New)
		var previous = _lookup.Values.ToDictionary(x => x.Id);

		var ids = snapshot.Select(x => x.Id).ToHashSet();

		// remove missing
		_items.RemoveWhere(x => !ids.Contains(x.Id));

		_lookup.Clear();

		foreach (var incoming in snapshot)
		{
			if (previous.TryGetValue(incoming.Id, out var old))
			{
				var merged = PreserveUserFlags(old, incoming);
				_items.Remove(old);
				_items.Add(merged);
				_lookup[merged.Id] = merged;
			}
			else
			{
				_items.Add(incoming);
				_lookup[incoming.Id] = incoming;
			}
		}

		LastSync = lastSync;
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


	protected virtual TModel PreserveUserFlags(TModel oldModel, TModel incoming) => incoming;
}