using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Base;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class IconService : IIconService
{
	private readonly Dictionary<string, string> _cache = [];
	private const string ResourcePrefix = "JakubKastner.MusicReleases.Icons";

	private static readonly Dictionary<Type, (IconType Type, string CssClass)> _meta =
	   new()
	   {
			{ typeof(SpotifyIcon), (IconType.Spotify, "icon-fill") },
			{ typeof(LucideIcon),  (IconType.Lucide,  "icon-stroke") },
			{ typeof(CustomIcon),  (IconType.Custom,  "icon-fill") },
	   };


	public string GetSvg<TIcon>(TIcon icon) where TIcon : Enum
	{
		if (!_meta.TryGetValue(typeof(TIcon), out var meta))
		{
			throw new ArgumentException($"Unsupported icon enum: {typeof(TIcon).Name}");
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
