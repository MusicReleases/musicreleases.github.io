using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Playlists;

public partial class ButtonPlaylist
{
	[Parameter, EditorRequired]
	public required SpotifyPlaylist Playlist { get; set; }

	[Parameter]
	public SpotifyRelease? Release { get; set; }

	private async Task AddToPlaylist(bool positionTop)
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

	private async Task RemoveFromPlaylist()
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
}