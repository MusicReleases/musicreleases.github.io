using IndexedDB.Blazor;
using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using Microsoft.JSInterop;

public class SpotifyReleasesDb : IndexedDb
{
	public SpotifyReleasesDb(IJSRuntime jSRuntime, string name, int version) : base(jSRuntime, name, version) { }

	public required IndexedSet<SpotifyReleaseEntity> Releases { get; set; }
	public IndexedSet<SpotifyArtistEntity> Artists { get; set; }
	public IndexedSet<SpotifyArtistReleaseEntity> ArtistsReleases { get; set; }
	//public IndexedSet<SpotifyTrackEntity> Tracks { get; set; }

	public IndexedSet<SpotifyLastUpdateEntity> Update { get; set; }
	public IndexedSet<SpotifyUserEntity> Users { get; set; }
}