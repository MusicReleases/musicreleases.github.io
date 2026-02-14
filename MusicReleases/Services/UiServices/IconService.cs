using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Services.UiServices;

public class IconService : IIconService
{
	private const string ResourcePrefix = "JakubKastner.MusicReleases.Icons";
	private const string iconFillStyle = "icon-fill";
	private const string iconStrokeStyle = "icon-stroke";

	private readonly Dictionary<string, string> _cache = [];

	private static readonly Dictionary<Type, (IconType Type, string CssClass)> _meta =
	   new()
	   {
			{ typeof(SpotifyIcon), (IconType.Spotify, iconFillStyle) },
			{ typeof(LucideIcon),  (IconType.Lucide,  iconStrokeStyle) },
			{ typeof(TablerIcon),  (IconType.Tabler,  iconStrokeStyle) },
			{ typeof(CustomIcon),  (IconType.Custom,  iconFillStyle) },
	   };


	public string GetSvg<TIcon>(TIcon icon) where TIcon : Enum
	{
		var runtimeType = icon.GetType();
		if (!_meta.TryGetValue(runtimeType, out var meta))
		{
			throw new ArgumentException($"Unsupported icon enum: {runtimeType.Name}");
		}

		var resourceName = GetResourcePath(meta.Type, icon);
		return LoadSvg(resourceName, meta.CssClass);
	}

	private static string GetResourcePath(IconType type, Enum icon)
	{
		var folder = type.ToString().ToKebabCase();
		var file = icon.ToString().ToKebabCase();
		return $"{ResourcePrefix}.{folder}.{file}.svg";
	}

	private string LoadSvg(string resourceName, string cssClass)
	{
		if (string.IsNullOrEmpty(resourceName))
		{
			return GetFallbackSvg();
		}

		if (_cache.TryGetValue(resourceName, out var cached))
		{
			return cached;
		}

		var assembly = typeof(IconService).Assembly;
		using var stream = assembly.GetManifestResourceStream(resourceName);

		if (stream == null)
		{
			return GetFallbackSvg();
		}

		using var reader = new StreamReader(stream);
		var svg = reader.ReadToEnd();

		svg = svg.Replace("<svg", $"<svg class='{cssClass}' aria-hidden='true' focusable='false'");

		_cache[resourceName] = svg;
		return svg;
	}

	private static string GetFallbackSvg()
	{
		return
			"<svg viewBox='0 0 24 24' class='icon-stroke' aria-hidden='true'>" +
				"<circle cx='12' cy='12' r='10'/>" +
				"<text x='12' y='16' text-anchor='middle' font-size='12'>?</text>" +
			"</svg>";
	}

}
