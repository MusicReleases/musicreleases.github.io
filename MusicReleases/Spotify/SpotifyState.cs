using JakubKastner.SpotifyApi.Artists;
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

	public void Merge(IEnumerable<TModel> items, DateTime lastSync)
	{
		_items ??= [];

		var previous = _lookup.Values.ToDictionary(a => a.Id);

		var newItems = items.ToList();
		var newItemIds = newItems.Select(i => i.Id).ToHashSet();

		// remove not existing
		_items.RemoveWhere(i => !newItemIds.Contains(i.Id));

		_lookup.Clear();

		foreach (var item in newItems)
		{
			if (previous.TryGetValue(item.Id, out var old))
			{
				// existing
				var merged = item;

				if (merged is SpotifyArtist newArtist && old is SpotifyArtist oldartist)
				{
					// merge artists new flag
					newArtist.New = oldartist.New;
				}

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
		StateChanged();

	}
}