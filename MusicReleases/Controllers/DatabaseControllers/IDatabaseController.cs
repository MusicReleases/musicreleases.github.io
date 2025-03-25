
using JakubKastner.MusicReleases.Base;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public interface IDatabaseController
{
	Task DeleteAll();
	IndexedDb GetDb();
	IndexedDbStore GetTable(IndexedDb db, Enums.DbStorageTablesSpotify tableName);
}