using JakubKastner.MusicReleases.Base;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyServiceOld
{
	Task DeleteAll();
	IndexedDb GetDb();
	IndexedDbStore GetTable(Enums.DbStorageTablesSpotify tableName, IndexedDb? db = null);
}