using static JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Base;

public static class Icons
{
    // TODO DI
    public static string? GetIconForRelease(ReleaseType releaseType)
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
