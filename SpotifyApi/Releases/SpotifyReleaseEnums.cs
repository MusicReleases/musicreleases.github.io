using static SpotifyAPI.Web.ArtistsAlbumsRequest;

namespace JakubKastner.SpotifyApi.Releases;

// TODO release group - all
public enum ReleaseGroup
{
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
	public static IncludeGroups GetApiReleaseGroup(ReleaseGroup releaseGroup)
	{
		return releaseGroup switch
		{
			//ReleaseType.All => throw new Exception("TODO"), // TODO release group - all
			ReleaseGroup.Albums => IncludeGroups.Album,
			ReleaseGroup.Tracks => IncludeGroups.Single,
			ReleaseGroup.Appears => IncludeGroups.AppearsOn,
			ReleaseGroup.Compilations => IncludeGroups.Compilation,
			ReleaseGroup.Podcasts => throw new NotImplementedException(), // TODO release group - podcasts
			_ => throw new Exception("Unsupported Release group"),
		};
	}

	public static ReleaseType MapReleaseTypeFromGroup(ReleaseGroup releaseGroup)
	{
		return releaseGroup switch
		{
			ReleaseGroup.Albums => ReleaseType.Album,
			ReleaseGroup.Tracks => ReleaseType.Track,
			ReleaseGroup.Compilations => ReleaseType.Compilation,
			ReleaseGroup.Podcasts => ReleaseType.Podcast,
			_ => throw new NotSupportedException(nameof(MapReleaseTypeFromGroup)),
		};
	}
	public static ArtistReleaseRole? MapReleaseRoleFromGroup(ReleaseGroup releaseGroup)
	{
		return releaseGroup switch
		{
			ReleaseGroup.Albums => ArtistReleaseRole.Main,
			ReleaseGroup.Tracks => ArtistReleaseRole.Main,
			ReleaseGroup.Appears => ArtistReleaseRole.Featured,
			ReleaseGroup.Compilations => ArtistReleaseRole.Main,
			ReleaseGroup.Podcasts => throw new NotImplementedException(), //TODO release group - podcasts;
			_ => throw new Exception("Unsupported Release group"),
		};
	}
}