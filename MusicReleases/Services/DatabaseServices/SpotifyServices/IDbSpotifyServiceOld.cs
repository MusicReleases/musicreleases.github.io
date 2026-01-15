using JakubKastner.MusicReleases.Enums;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyServiceOld
{
	Task DeleteAll();
	IndexedDb GetDb();
	IndexedDbStore GetTable(DbStorageTablesSpotify tableName, IndexedDb? db = null);
}