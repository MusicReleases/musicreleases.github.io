using JakubKastner.MusicReleases.Base;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Database;

public static class SpotifyReleasesDb
{
	public static readonly string Name = "SpotifyReleases";
	public static readonly int Version = 2;
	public static IEnumerable<string> GetAllTables()
	{
		return EnumUtil.GetNames<DbStorageTablesSpotify>();
	}
}