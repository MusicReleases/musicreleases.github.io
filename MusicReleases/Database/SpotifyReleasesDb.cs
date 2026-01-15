using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Database;

public static class SpotifyReleasesDb
{
	public static readonly string Name = "SpotifyReleases";
	public static readonly int Version = 3;
	public static IEnumerable<string> GetAllTables()
	{
		return EnumExtensions.GetNames<DbStorageTablesSpotify>();
	}
}