using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace JakubKastner.MusicReleases.Services.UiServices;

public sealed class NavigationService : INavigationService
{
	private readonly NavigationManager _nav;

	// reentrancy guard
	private int _navDepth;

	public string Current => _nav.ToBaseRelativePath(_nav.Uri);

	public event Action<string>? Navigated;

	public NavigationService(NavigationManager nav)
	{
		_nav = nav;
		_nav.LocationChanged += OnLocationChanged;
	}

	public void Dispose()
	{
		_nav.LocationChanged -= OnLocationChanged;
	}

	public void Navigate(string url, bool forceLoad = false, bool replace = false, string? reason = null)
	{
		Console.WriteLine($"[NAV] {Current} -> {url} replace={replace} forceLoad={forceLoad} reason={reason}\n{Environment.StackTrace}");

		// guard against infinite loops - if something goes wrong with navigation, we dont want to crash the app
		if (_navDepth > 8)
		{
			Console.WriteLine("[NAV][WARN] Too deep navigation recursion, aborting.");
			return;
		}

		var current = Current.TrimEnd('/');
		var target = url.TrimStart('/').TrimEnd('/');


		if (string.Equals(current, target, StringComparison.OrdinalIgnoreCase))
		{
			Console.WriteLine($"[NAV][SKIP] {current} -> {target} (same url) reason={reason}");
			return;
		}

		try
		{
			_navDepth++;
			_nav.NavigateTo(url, forceLoad, replace);
		}
		finally
		{
			_navDepth--;
		}
	}

	private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
	{
		Console.WriteLine($"[NAV][CHANGED] -> {_nav.ToBaseRelativePath(e.Location)} intercepted={e.IsNavigationIntercepted}");
		Navigated?.Invoke(e.Location);
	}


	private string? _popupBackgroundUrl;
	private bool _popupActive;

	public bool IsPopupActive => _popupActive;
	public string? PopupBackgroundUrl => _popupBackgroundUrl;

	public void OpenPopup(string popupUrl, string reason)
	{
		var current = "/" + Current;

		if (!_popupActive)
		{
			_popupBackgroundUrl = current;
			_popupActive = true;
		}

		// popup -> popup = replace
		Navigate(popupUrl, false, true, $"OpenPopup:{reason}");
	}

	public void ClosePopup(string fallback = "/releases")
	{
		var target = _popupBackgroundUrl ?? fallback;

		_popupActive = false;
		_popupBackgroundUrl = null;

		Navigate(target, replace: true, reason: "ClosePopup");
	}

}
