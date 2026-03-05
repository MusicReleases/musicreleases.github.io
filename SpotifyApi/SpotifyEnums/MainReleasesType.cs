using static SpotifyAPI.Web.ArtistsAlbumsRequest;

namespace JakubKastner.SpotifyApi.SpotifyEnums;

// TODO all?????
public enum MainReleasesType
{
	//All,
	Albums,
	Tracks,
	Appears,
	Compilations,
	Podcasts,
}

public enum ReleaseType
{
	Album,
	Track,
	Compilation,
	Podcast,
}

public enum ArtistReleaseRole
{
	Main,
	Featured,
}

public static class EnumReleaseTypeExtensions
{
	public static HashSet<MainReleasesType> GetAllReleaseTypes()
	{
		return [.. Enum.GetValues<MainReleasesType>().Cast<MainReleasesType>()];
	}

	public static IncludeGroups GetApiReleaseType(MainReleasesType releaseType)
	{
		return releaseType switch
		{
			//ReleaseType.All => throw new Exception("TODO"), // TODO all
			MainReleasesType.Albums => IncludeGroups.Album,
			MainReleasesType.Tracks => IncludeGroups.Single,
			MainReleasesType.Appears => IncludeGroups.AppearsOn,
			MainReleasesType.Compilations => IncludeGroups.Compilation,
			MainReleasesType.Podcasts => throw new NotImplementedException(), //TODO podcasts;
			_ => throw new Exception("Unsupported Release Type"),
		};
	}

	public static ReleaseType MapFromMain(MainReleasesType mainReleaseType)
	{
		return mainReleaseType switch
		{
			MainReleasesType.Albums => ReleaseType.Album,
			MainReleasesType.Tracks => ReleaseType.Track,
			MainReleasesType.Compilations => ReleaseType.Compilation,
			MainReleasesType.Podcasts => ReleaseType.Podcast,
			_ => throw new NotSupportedException(nameof(MapFromMain)),
		};
	}
	public static ArtistReleaseRole MapReleaseRole(MainReleasesType mainReleasesType)
	{
		return mainReleasesType == MainReleasesType.Appears ? ArtistReleaseRole.Featured : ArtistReleaseRole.Main;
	}
}