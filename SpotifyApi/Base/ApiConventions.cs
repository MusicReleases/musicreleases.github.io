namespace JakubKastner.SpotifyApi.Base;

internal static class ApiConventions
{
	public const string ExternalUrlSpotifyKey = "spotify";

	public const int SmallImageIndex = 0;
	public const int MediumImageIndex = 1;
	public const int LargeImageIndex = 2;

	public static readonly IReadOnlyDictionary<string, ReleaseType> ReleaseTypeMap = new Dictionary<string, ReleaseType>()
	{
		["album"] = ReleaseType.Album,
		["single"] = ReleaseType.Track,
		["compilation"] = ReleaseType.Compilation,
	};
}
