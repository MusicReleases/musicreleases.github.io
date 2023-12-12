using static SpotifyAPI.Web.ArtistsAlbumsRequest;

namespace JakubKastner.SpotifyApi.Base;

public static class SpotifyEnums
{
	// TODO all?????
	public enum ReleaseType
	{
		//All,
		Albums,
		Tracks,
		Appears,
		Compilations,
		Podcasts,
	}

	public static HashSet<ReleaseType> GetAllReleaseTypes()
	{
		return Enum.GetValues(typeof(ReleaseType)).Cast<ReleaseType>().ToHashSet();
	}

	public static IncludeGroups GetApiReleaseType(ReleaseType releaseType)
	{
		return releaseType switch
		{
			//ReleaseType.All => throw new Exception("TODO"), // TODO all
			ReleaseType.Albums => IncludeGroups.Album,
			ReleaseType.Tracks => IncludeGroups.Single,
			ReleaseType.Appears => IncludeGroups.AppearsOn,
			ReleaseType.Compilations => IncludeGroups.Compilation,
			ReleaseType.Podcasts => throw new NotImplementedException(), //TODO podcasts;
			_ => throw new Exception("Unsupported Release Type"),
		};
	}
}
