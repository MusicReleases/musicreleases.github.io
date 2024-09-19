using IndexedDB.Blazor;
using JakubKastner.MusicReleases.Entities.Api.Spotify;
using Microsoft.JSInterop;

public class SpotifyReleasesDb : IndexedDb
{
	public SpotifyReleasesDb(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version) { }

	public IndexedSet<SpotifyReleaseEntity> Releases { get; set; }
	public IndexedSet<SpotifyArtistEntity> Artists { get; set; }
	public IndexedSet<SpotifyArtistReleaseEntity> ArtistsReleases { get; set; }
	//public IndexedSet<SpotifyTrackEntity> Tracks { get; set; }

	//public SpotifyLastUpdateEntity Update { get; set; }
}