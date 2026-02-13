namespace JakubKastner.MusicReleases.Services.UiServices;

public interface IIconService
{
	string GetSvg<TIcon>(TIcon icon) where TIcon : Enum;
}