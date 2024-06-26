﻿using Fluxor;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

public class SpotifyPlaylistsFeature : Feature<SpotifyPlaylistsState>
{
	public override string GetName() => "ApiSpotifyPlaylists";

	protected override SpotifyPlaylistsState GetInitialState()
	{
		return new()
		{
			Initialized = false,
			Loading = false,
			Playlists = new SpotifyUserList<SpotifyPlaylist>(),
		};
	}
}
