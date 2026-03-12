using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseTrack : IDisposable
{
	[Inject]
	private IDragDropService DragDropService { get; set; } = default!;

	[Inject]
	private ISettingsService SettingsService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required SpotifyTrack Track { get; set; }


	private string PlaylistButtonTitle => _isPlaylistListDisplayed ? "Hide playlists" : "Add track to playlist";


	private const string _buttonClass = "track-info";

	private const string _buttonClassAction = $"{_buttonClass} action";

	private const string _nameButtonClass = $"{_buttonClass} name";

	private bool _renderPlaylistList = false;

	private bool _isPlaylistListDisplayed = false;


	protected override void OnInitialized()
	{
		SettingsService.OnChange += StateChanged;
	}

	public void Dispose()
	{
		SettingsService.OnChange -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private void OnDragStart()
	{
		DragDropService.StartDrag(Track, DragDropType.Track);
	}

	private void TogglePlaylists()
	{
		if (!_renderPlaylistList)
		{
			_renderPlaylistList = true;
		}

		_isPlaylistListDisplayed = !_isPlaylistListDisplayed;
	}
}
