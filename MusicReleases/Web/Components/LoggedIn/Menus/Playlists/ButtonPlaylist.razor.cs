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

	private async Task AddToPlaylist(bool positionTop)
	{
		await AddReleaseToPlaylist(positionTop);
		await AddTrackToPlaylist(positionTop);
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

		Playlist = await SpotifyPlaylistsService.AddTracks(Playlist, Release.Tracks, positionTop);
	}

	private async Task AddTrackToPlaylist(bool positionTop)
	{
		if (Track is null)
		{
			return;
		}

		Playlist = await SpotifyPlaylistsService.AddTrack(Playlist, Track, positionTop);
	}

	private async Task RemoveFromPlaylist()
	{
		await RemoveReleaseFromPlaylist();
		await RemoveTrackFromPlaylist();
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
		Playlist = await SpotifyPlaylistsService.RemoveTracks(Playlist, Release.Tracks);
	}

	private async Task RemoveTrackFromPlaylist()
	{
		if (Track is null)
		{
			return;
		}
		Playlist = await SpotifyPlaylistsService.RemoveTrack(Playlist, Track);
	}
}