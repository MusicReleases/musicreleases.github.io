using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Objects;

public class SpotifyFilter
{
	public ReleaseType ReleaseType { get; set; } = ReleaseType.Albums;
}
