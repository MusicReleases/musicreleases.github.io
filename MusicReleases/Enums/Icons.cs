using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Enums;

public enum IconType
{
	Spotify,
	Lucide,
	Custom,
}

public enum SpotifyIcon
{
	SmallGreen,
}

public enum LucideIcon
{
	ArrowDownAZ,
	ArrowDownUp,
	ArrowDownZA,
	Calendar,
	CirclePlus,
	Delete,
	Disc3,
	DiscAlbum,
	Flame,
	Funnel,
	FunnelX,
	House,
	ChevronDown,
	ChevronUp,
	ListMusic,
	ListOrdered,
	LoaderCircle,
	LogOut,
	MicVocal,
	Minus,
	Music,
	Play,
	Plus,
	Podcast,
	RefreshCcw,
	Settings,
	UserRound,
	Users,
	X,
}

public enum CustomIcon
{
	Logo,
}

public static class EnumIconsExtensions
{
	public static Enum GetIconForServiceType(ServiceType serviceType)
	{
		return serviceType switch
		{
			ServiceType.Spotify => SpotifyIcon.SmallGreen,
			_ => throw new NotSupportedException(nameof(serviceType)),
		};
	}

	public static LucideIcon GetIconForRelease(ReleaseType releaseType)
	{
		return releaseType switch
		{
			ReleaseType.Albums => LucideIcon.Disc3,
			ReleaseType.Tracks => LucideIcon.Music,
			ReleaseType.Appears => LucideIcon.Users,
			ReleaseType.Compilations => LucideIcon.DiscAlbum,
			ReleaseType.Podcasts => LucideIcon.Podcast,
			_ => throw new NotSupportedException(nameof(releaseType)),
		};
	}
	public static string GetIconForReleaseOld(ReleaseType releaseType)
	{
		return releaseType switch
		{
			ReleaseType.Albums => "compact-disc",
			ReleaseType.Tracks => "music",
			ReleaseType.Appears => "user-friends",
			ReleaseType.Compilations => "th-large",
			ReleaseType.Podcasts => "podcast",
			_ => throw new NotSupportedException(nameof(releaseType)),
		};
	}
}