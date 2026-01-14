using System.Reflection;
using static JakubKastner.MusicReleases.Base.Icons;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public class IconService : IIconService
{
	private readonly Dictionary<string, string> _cache = [];
	private const string ProjectName = "JakubKastner.MusicReleases";

	public string GetSvg(LucideIcon icon)
	{
		var resourceName = icon switch
		{
			//LucideIcon.Search => $"{ProjectName}.Icons.lucide.search.svg",
			_ => ""
		};
		return LoadSvg(resourceName, "icon-stroke");
	}

	public string GetSvg(SpotifyIcon icon)
	{
		var resourceName = icon switch
		{
			SpotifyIcon.SmallGreen => $"{ProjectName}.Icons.spotify.small-green.svg",
			_ => ""
		};
		return LoadSvg(resourceName, "icon-fill");
	}


	public string GetSvg(CustomIcon icon)
	{
		var resourceName = icon switch
		{
			//CustomIcon.Logo => $"{ProjectName}.Icons.custom.logo.svg",
			_ => ""
		};
		return LoadSvg(resourceName, "icon-fill");
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

		var assembly = Assembly.GetExecutingAssembly();
		using var stream = assembly.GetManifestResourceStream(resourceName);

		if (stream == null)
		{
			return GetFallbackSvg();
		}

		using var reader = new StreamReader(stream);
		var svg = reader.ReadToEnd();

		svg = svg.Replace("<svg", $"<svg class='{cssClass}'");

		_cache[resourceName] = svg;
		return svg;
	}

	private static string GetFallbackSvg()
	{
		return "<svg viewBox='0 0 24 24' class='icon-stroke'><circle cx='12' cy='12' r='10'/><text x='12' y='16' text-anchor='middle'>?</text></svg>";
	}

}
