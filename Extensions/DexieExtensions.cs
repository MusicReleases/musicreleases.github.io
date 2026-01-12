using DexieNET;

namespace JakubKastner.Extensions;

public static class DexieExtensions
{
	public static async Task PutSafe<T, TKey>(this Table<T, TKey> table, T item) where T : IDBStore
	{
		await table.BulkPut([item]);
	}

	public static async Task BulkPutSafe<T, TKey>(this Table<T, TKey> table, IEnumerable<T> items) where T : IDBStore
	{
		if (items is ICollection<T> collection && collection.Count == 0)
		{
			return;
		}
		if (items is IReadOnlyCollection<T> readOnlyCol && readOnlyCol.Count == 0)
		{
			return;
		}

		var itemsArray = items as T[] ?? [.. items];

		if (itemsArray.Length > 0)
		{
			await table.BulkPut(itemsArray);
		}
	}

	public static async Task BulkDeleteSafe<T, TKey>(this Table<T, TKey> table, IEnumerable<TKey> keys) where T : IDBStore
	{
		if (keys is ICollection<T> collection && collection.Count == 0)
		{
			return;
		}
		if (keys is IReadOnlyCollection<T> readOnlyCol && readOnlyCol.Count == 0)
		{
			return;
		}

		var keysArray = keys as TKey[] ?? [.. keys];

		if (keysArray.Length > 0)
		{
			await table.BulkDelete(keysArray);
		}
	}
}