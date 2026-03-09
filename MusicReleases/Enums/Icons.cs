using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Enums;

public enum IconType
{
	Spotify,
	Lucide,
	Tabler,
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
	Clock,
	Cog,
	Delete,
	Disc3,
	DiscAlbum,
	Dot,
	Ellipsis,
	Eye,
	EyeOff,
	Flame,
	Funnel,
	FunnelX,
	Globe,
	Heart,
	House,
	ChevronDown,
	ChevronUp,
	ListCheck,
	ListMusic,
	ListOrdered,
	ListTodo,
	LoaderCircle,
	Lock,
	LogOut,
	Menu,
	MicVocal,
	Minus,
	Moon,
	Music,
	Music2,
	Palette,
	Play,
	Plus,
	Podcast,
	RefreshCcw,
	Settings,
	Sun,
	UserRound,
	UserRoundCog,
	Users,
	X,
}

public enum TablerIcon
{
	Explicit,
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

	public static LucideIcon GetIconForReleaseType(ReleaseType releaseType)
	{
		return releaseType switch
		{
			ReleaseType.Album => LucideIcon.Disc3,
			ReleaseType.Track => LucideIcon.Music,
			ReleaseType.Compilation => LucideIcon.DiscAlbum,
			ReleaseType.Podcast => LucideIcon.Podcast,
			_ => throw new NotSupportedException(nameof(releaseType)),
		};
	}

	public static LucideIcon GetIconForRelease(ReleaseGroup mainReleasesType)
	{
		return mainReleasesType switch
		{
			ReleaseGroup.Albums => LucideIcon.Disc3,
			ReleaseGroup.Tracks => LucideIcon.Music,
			ReleaseGroup.Appears => LucideIcon.Users,
			ReleaseGroup.Compilations => LucideIcon.DiscAlbum,
			ReleaseGroup.Podcasts => LucideIcon.Podcast,
			_ => throw new NotSupportedException(nameof(mainReleasesType)),
		};
	}
}