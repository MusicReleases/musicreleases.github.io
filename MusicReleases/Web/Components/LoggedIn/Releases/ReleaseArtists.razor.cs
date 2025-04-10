﻿using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseArtists
{
	[Parameter, EditorRequired]
	public required HashSet<SpotifyArtist> ArtistsObj { get; set; }
}
