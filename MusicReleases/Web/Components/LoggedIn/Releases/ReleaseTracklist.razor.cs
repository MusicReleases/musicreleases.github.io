using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Releases;

public partial class ReleaseTracklist
{
	[Parameter, EditorRequired]
	public required SortedSet<SpotifyTrack> Tracks { get; set; }


	private int DiscsCount => Tracks.Select(t => t.DiscNumber).Distinct().Count();


	private int? _lastDisc = null;
}
