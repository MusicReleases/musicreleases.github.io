using static SpotifyAPI.Web.ArtistsAlbumsRequest;

namespace JakubKastner.SpotifyApi.SpotifyEnums;

// TODO all?????
public enum ReleaseGroup
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
	public static IncludeGroups GetApiReleaseGroup(ReleaseGroup releaseType)
	{
		return releaseType switch
		{
			//ReleaseType.All => throw new Exception("TODO"), // TODO all
			ReleaseGroup.Albums => IncludeGroups.Album,
			ReleaseGroup.Tracks => IncludeGroups.Single,
			ReleaseGroup.Appears => IncludeGroups.AppearsOn,
			ReleaseGroup.Compilations => IncludeGroups.Compilation,
			ReleaseGroup.Podcasts => throw new NotImplementedException(), //TODO podcasts;
			_ => throw new Exception("Unsupported Release Type"),
		};
	}

	public static ReleaseType MapReleaseTypeFromGroup(ReleaseGroup mainReleaseType)
	{
		return mainReleaseType switch
		{
			ReleaseGroup.Albums => ReleaseType.Album,
			ReleaseGroup.Tracks => ReleaseType.Track,
			ReleaseGroup.Compilations => ReleaseType.Compilation,
			ReleaseGroup.Podcasts => ReleaseType.Podcast,
			_ => throw new NotSupportedException(nameof(MapReleaseTypeFromGroup)),
		};
	}
	public static ArtistReleaseRole MapReleaseRoleFromGroup(ReleaseGroup mainReleasesType)
	{
		return mainReleasesType == ReleaseGroup.Appears ? ArtistReleaseRole.Featured : ArtistReleaseRole.Main;
	}
}