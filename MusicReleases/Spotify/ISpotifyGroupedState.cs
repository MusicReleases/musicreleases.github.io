using JakubKastner.SpotifyApi.Objects.Base;
using System.Collections.Concurrent;

namespace JakubKastner.MusicReleases.Spotify;

internal interface ISpotifyGroupedState<TModel, TGroupKey>
	where TModel : SpotifyIdNameObject
	where TGroupKey : notnull
{
	ConcurrentDictionary<TGroupKey, IReadOnlySet<TModel>> Items { get; }
	ConcurrentDictionary<TGroupKey, DateTime> LastSync { get; }

	event Action? OnChange;

	void Set(TGroupKey group, IEnumerable<TModel> items, DateTime lastSync);
}