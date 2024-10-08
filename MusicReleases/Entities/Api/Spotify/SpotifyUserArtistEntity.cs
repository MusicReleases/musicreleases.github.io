﻿
using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyUserArtistEntity : SpotifyGuidEntity
{
	public string? UserId { get; set; }

	public string? ArtistId { get; set; }
}
