using JakubKastner.MusicReleases.Objects;
using JakubKastner.SpotifyApi.Base;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISpotifyFilterService
{
	string ClearFilter(MenuButtonsType type);
	string GetFilterUrl();
	string GetFilterUrl(int? year);
	string GetFilterUrl(int? year, int? month);
	string GetFilterUrl(SpotifyEnums.ReleaseType releaseType);
	string GetFilterUrl(string? artist);
	SpotifyFilter ParseFilterUrl(string? releaseType, string? year, string? month, string? artist);
}