using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class ButtonPlaylist
{
	[Parameter, EditorRequired]
	public required SpotifyPlaylist Playlist { get; set; }

	[Parameter]
	public SpotifyRelease? Release { get; set; }

	[Parameter]
	public SpotifyTrack? Track { get; set; }

	private bool _isWorking;

	private void RefreshPlaylistData()
	{
		var fresh = SpotifyPlaylistState.GetById(Playlist.Id);
		if (fresh != null)
		{
			Console.WriteLine($"DEBUG: Refreshing playlist. Old tracks: {Playlist.Tracks.Count}, New tracks: {fresh.Tracks.Count}");
			Playlist = fresh;
		}

		else
		{
			Console.WriteLine("DEBUG: Playlist not found in State!");
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
		await SpotifyPlaylistService.AddTracks(Playlist.UrlApp, [Track], positionTop);
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
		await SpotifyPlaylistService.RemoveTracks(Playlist.Id, [Track]);
	}
}