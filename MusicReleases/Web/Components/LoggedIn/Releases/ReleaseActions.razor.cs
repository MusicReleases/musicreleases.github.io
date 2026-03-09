using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseActions
{
	[Inject]
	private ISpotifyTrackService SpotifyTracksService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required SpotifyRelease SpotifyRelease { get; set; }


	private const string _buttonClass = "release-actions";

	private const ReleaseListComponent _listPlaylist = ReleaseListComponent.PlaylistRelease;

	private const ReleaseListComponent _listTracklist = ReleaseListComponent.Track;

	private ReleaseListComponent? _activeList = null;

	private ReleaseListComponent? _loadingList = null;

	private readonly HashSet<ReleaseListComponent> _renderedLists = [];


	private async Task ToggleList(ReleaseListComponent listType)
	{
		if (_loadingList is not null)
		{
			return;
		}

		if (_activeList == listType)
		{
			_activeList = null;
			return;
		}

		_loadingList = listType;
		StateHasChanged();

		try
		{
			await GetTracks();

			_renderedLists.Add(listType);
			_activeList = listType;
		}
		finally
		{
			_loadingList = null;
		}
	}

	private bool RenderList(ReleaseListComponent listType)
	{
		return _renderedLists.Contains(listType);
	}

	private bool IsListActive(ReleaseListComponent listType)
	{
		return _activeList == listType;
	}

	private bool IsListLoading(ReleaseListComponent listType)
	{
		return _loadingList == listType;
	}

	private string ListButtonTitle(ReleaseListComponent listType)
	{
		var active = IsListActive(listType);

		return listType switch
		{
			ReleaseListComponent.Track => active ? "Hide tracklist" : "View tracklist",
			ReleaseListComponent.PlaylistRelease => active ? "Hide playlists" : "Add release to playlist",
			_ => throw new NotImplementedException(),
		};
	}

	private LucideIcon ListButtonIcon(ReleaseListComponent listType)
	{
		var isLoading = IsListLoading(listType);

		return listType switch
		{
			ReleaseListComponent.Track => isLoading ? LucideIcon.LoaderCircle : LucideIcon.ListMusic,
			ReleaseListComponent.PlaylistRelease => isLoading ? LucideIcon.LoaderCircle : LucideIcon.Plus,
			_ => throw new NotImplementedException(),
		};
	}

	private async Task GetTracks()
	{
		if (SpotifyRelease.Tracks?.Count > 0)
		{
			// tracks loaded
			return;
		}
		await SpotifyTracksService.Get(SpotifyRelease);
	}
}
