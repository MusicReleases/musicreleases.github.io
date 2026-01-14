using JakubKastner.MusicReleases.Base;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface IIconService
{
	string GetSvg(Icons.CustomIcon icon);
	string GetSvg(Icons.LucideIcon icon);
	string GetSvg(Icons.SpotifyIcon icon);
}