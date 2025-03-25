using JakubKastner.MusicReleases.Base;
using static JakubKastner.MusicReleases.Base.Enums;

public static class SpotifyReleasesDb
{
	public static readonly string Name = "MusicReleases";
	public static readonly int Version = 1;
	public static IEnumerable<string> GetAllTables()
	{
		return EnumUtil.GetNames<DbStorageTablesSpotify>();
	}
}