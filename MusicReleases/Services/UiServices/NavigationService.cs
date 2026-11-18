using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace JakubKastner.MusicReleases.Services.UiServices;

public sealed class NavigationService : INavigationService
{
	private readonly NavigationManager _nav;

	// reentrancy guard
	private int _navDepth;

	public string CurrentUrl => _nav.ToBaseRelativePath(_nav.Uri);
	public string BaseUrl => _nav.BaseUri;
	public Uri CurrentUri => new(_nav.Uri);


	public event Action<string>? Navigated;

	public NavigationService(NavigationManager nav)
	{
		_nav = nav;
		_nav.LocationChanged += OnLocationChanged;

		SyncPopupState();
	}

	public void Dispose()
	{
		_nav.LocationChanged -= OnLocationChanged;
	}

	public void Navigate(string url, bool forceLoad = false, bool replace = false, string? reason = null)
	{
		//Console.WriteLine($"[NAV] {Current} -> {url} replace={replace} forceLoad={forceLoad} reason={reason}\n{Environment.StackTrace}");
		Console.WriteLine($"[NAV] {CurrentUrl} -> {url} replace={replace} forceLoad={forceLoad} reason={reason}");

		// guard against infinite loops - if something goes wrong with navigation, we dont want to crash the app
		if (_navDepth > 8)
		{
			Console.WriteLine("[NAV][WARN] Too deep navigation recursion, aborting.");
			return;
		}

		var current = CurrentUrl.TrimEnd('/');
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

		SyncPopupState();

		Navigated?.Invoke(e.Location);
	}


	private string? _popupBackgroundUrl;
	private bool _popupActive;

	public bool IsPopupActive => _popupActive;
	public string? PopupBackgroundUrl => _popupBackgroundUrl;

	private void SyncPopupState()
	{
		bool isPopup = IsPopupPath();

		if (!isPopup)
		{
			_popupActive = false;
			_popupBackgroundUrl = null;
			return;
		}

		if (!_popupActive)
		{
			_popupActive = true;
			_popupBackgroundUrl ??= "/releases";
		}
	}

	private bool IsPopupPath()
	{
		var cleaned = CurrentUrl.TrimStart('/').ToLowerInvariant();
		var comparisonType = StringComparison.OrdinalIgnoreCase;

		return cleaned.StartsWith("tasks", comparisonType)
			|| cleaned.StartsWith("settings", comparisonType);
	}

	public void OpenPopup(string popupUrl, string reason)
	{
		if (!_popupActive)
		{
			if (IsPopupPath())
			{
				_popupBackgroundUrl ??= "/releases";
			}
			else
			{
				_popupBackgroundUrl = $"/{CurrentUrl}";
			}
			_popupActive = true;
		}

		// popup -> popup = replace
		Navigate(popupUrl, reason: $"OpenPopup:{reason}");
	}

	public void ClosePopup(string fallback = "/releases")
	{
		var target = _popupBackgroundUrl ?? fallback;

		_popupActive = false;
		_popupBackgroundUrl = null;

		Navigate(target, reason: "ClosePopup");
	}

}
