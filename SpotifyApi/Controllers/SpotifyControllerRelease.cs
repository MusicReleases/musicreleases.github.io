﻿using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerRelease : ISpotifyControllerRelease
{
	private readonly IControllerApiRelease _controllerApiRelease;
	private readonly ISpotifyControllerArtist _controllerArtist;

	public SpotifyControllerRelease(IControllerApiRelease controllerApiRelease, ISpotifyControllerArtist controllerArtist)
	{
		_controllerApiRelease = controllerApiRelease;
		_controllerArtist = controllerArtist;
	}

	// get all relases for user followed artist
	public async Task<ISet<SpotifyAlbum>> GetAllUserFollowedArtistsReleases(ReleaseType releaseType = ReleaseType.Albums)
	{
		var artists = await _controllerArtist.GetUserFollowedArtists();
		SortedSet<SpotifyAlbum> releases = new();

		foreach (var artist in artists)
		{
			// TODO save release to artist releases list
			var releasesFromApi = await _controllerApiRelease.GetArtistReleasesFromApi(artist.Id, releaseType);
			// TODO try again
			releasesFromApi ??= await _controllerApiRelease.GetArtistReleasesFromApi(artist.Id, releaseType);
			releases.UnionWith(releasesFromApi);
		}

		return releases;
	}
}