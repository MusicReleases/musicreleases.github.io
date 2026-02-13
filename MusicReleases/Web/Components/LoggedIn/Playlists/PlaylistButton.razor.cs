using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Playlists;

public partial class PlaylistButton
{
	[Parameter, EditorRequired]
	public required SpotifyPlaylist Playlist { get; set; }

	[Parameter]
	public SpotifyRelease? Release { get; set; }

	[Parameter]
	public SpotifyTrack? Track { get; set; }

	private bool IsReleaseInPlaylist => Release?.Tracks is not null && Release.Tracks.Any(track => Playlist.Tracks.Contains(track.Id));
	private bool IsTrackInPlaylist => Track is not null && Playlist.Tracks.Contains(Track.Id);
	private bool IsInPlaylist => IsReleaseInPlaylist || IsTrackInPlaylist;

	private bool _isWorking;

	protected override void OnParametersSet()
	{
		if (Release is null && Track is null)
		{
			throw new InvalidOperationException($"You must provide either {nameof(Release)} or {nameof(Track)}.");
		}

		if (Release is not null && Track is not null)
		{
			throw new InvalidOperationException($"You must provide only {nameof(Release)} or {nameof(Track)}, not both.");
		}
	}

	private string Title(bool positionTop)
	{
		string itemName = Release is not null ? $"release '{Release.Name}'" : Track is not null ? $"track '{Track.Name}'" : "_";

		if (IsInPlaylist)
		{
			return $"Remove {itemName} from playlist '{Playlist.Name}'";
		}

		var actionName = positionTop ? "top" : "end";
		return $"Add {itemName} to {actionName} of playlist '{Playlist.Name}'";
	}

	private void RefreshPlaylistData()
	{
		var fresh = SpotifyPlaylistState.GetById(Playlist.Id);
		if (fresh != null)
		{
			Playlist = fresh;
		}
	}

	private async Task PlaylistAction()
	{
		if (IsInPlaylist)
		{
			await RemoveFromPlaylist();
		}
		else
		{
			// TODO settings: default position when adding to playlist (top/bottom)
			await AddToPlaylist(false);
		}
	}


	private async Task AddToPlaylist(bool positionTop)
	{
		try
		{
			await AddReleaseToPlaylist(positionTop);
			await AddTrackToPlaylist(positionTop);

			RefreshPlaylistData();
		}
		finally
		{
			_isWorking = false;
			StateHasChanged();
		}
	}

	private async Task AddReleaseToPlaylist(bool positionTop)
	{
		if (Release is null)
		{
			return;
		}

		if (Release.Tracks is null || Release.Tracks.Count < 1)
		{
			// try to get tracks
			await SpotifyTracksService.Get(Release);
		}

		if (Release.Tracks is null || Release.Tracks.Count < 1)
		{
			return;
		}

		await SpotifyPlaylistService.AddTracks(Playlist.Id, Release.Tracks, positionTop);
	}

	private async Task AddTrackToPlaylist(bool positionTop)
	{
		if (Track is null)
		{
			return;
		}
		await SpotifyPlaylistService.AddTrack(Playlist.Id, Track, positionTop);
	}

	private async Task RemoveFromPlaylist()
	{
		_isWorking = true;
		try
		{
			await RemoveReleaseFromPlaylist();
			await RemoveTrackFromPlaylist();

			RefreshPlaylistData();
		}
		finally
		{
			_isWorking = false;
			StateHasChanged();
		}
	}

	private async Task RemoveReleaseFromPlaylist()
	{
		if (Release is null)
		{
			return;
		}
		if (Release.Tracks is null || Release.Tracks.Count < 1)
		{
			// try to get tracks
			await SpotifyTracksService.Get(Release);
		}
		if (Release.Tracks is null || Release.Tracks.Count < 1)
		{
			return;
		}

		await SpotifyPlaylistService.RemoveTracks(Playlist.Id, Release.Tracks);
	}

	private async Task RemoveTrackFromPlaylist()
	{
		if (Track is null)
		{
			return;
		}
		await SpotifyPlaylistService.RemoveTrack(Playlist.Id, Track);
	}
}