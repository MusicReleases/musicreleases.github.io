using static JakubKastner.SpotifyApi.Enums;

namespace MusicReleases.Base;

public static class Icons
{
    public static string? GetReleases(ReleaseType releaseType)
    {
        return releaseType switch
        {
            ReleaseType.Albums => "compact-disc",
            ReleaseType.Tracks => "music",
            ReleaseType.Appears => "user-friends",
            ReleaseType.Compilations => "th-large",
            ReleaseType.Podcasts => "podcast",
            _ => string.Empty,
        };
    }
}
