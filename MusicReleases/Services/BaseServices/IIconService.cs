namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface IIconService
{
	string GetSvg<TIcon>(TIcon icon) where TIcon : Enum;
}