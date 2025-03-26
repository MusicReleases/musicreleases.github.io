using JakubKastner.MusicReleases.Base;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers;

public interface IDatabaseController
{
	Task DeleteAll();
	IndexedDb GetDb();
	IndexedDbStore GetTable(Enums.DbStorageTablesSpotify tableName, IndexedDb? db = null);
}