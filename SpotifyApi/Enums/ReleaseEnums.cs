using static SpotifyAPI.Web.ArtistsAlbumsRequest;

namespace JakubKastner.SpotifyApi.Enums;

// TODO all?????
public enum ReleaseEnums
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
	public static IncludeGroups GetApiReleaseGroup(ReleaseEnums releaseType)
	{
		return releaseType switch
		{
			//ReleaseType.All => throw new Exception("TODO"), // TODO all
			ReleaseEnums.Albums => IncludeGroups.Album,
			ReleaseEnums.Tracks => IncludeGroups.Single,
			ReleaseEnums.Appears => IncludeGroups.AppearsOn,
			ReleaseEnums.Compilations => IncludeGroups.Compilation,
			ReleaseEnums.Podcasts => throw new NotImplementedException(), //TODO podcasts;
			_ => throw new Exception("Unsupported Release Type"),
		};
	}

	public static ReleaseType MapReleaseTypeFromGroup(ReleaseEnums mainReleaseType)
	{
		return mainReleaseType switch
		{
			ReleaseEnums.Albums => ReleaseType.Album,
			ReleaseEnums.Tracks => ReleaseType.Track,
			ReleaseEnums.Compilations => ReleaseType.Compilation,
			ReleaseEnums.Podcasts => ReleaseType.Podcast,
			_ => throw new NotSupportedException(nameof(MapReleaseTypeFromGroup)),
		};
	}
	public static ArtistReleaseRole MapReleaseRoleFromGroup(ReleaseEnums mainReleasesType)
	{
		return mainReleasesType == ReleaseEnums.Appears ? ArtistReleaseRole.Featured : ArtistReleaseRole.Main;
	}
}