using IndexedDB.Blazor;
using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using Microsoft.JSInterop;

public class SpotifyReleasesDbOld(IJSRuntime jSRuntime, string name, int version) : IndexedDb(jSRuntime, name, version)
{
	public required IndexedSet<SpotifyUserEntity> Users { get; set; }
	public required IndexedSet<SpotifyLastUpdateEntity> Updates { get; set; }

	public required IndexedSet<SpotifyUserArtistEntity> UsersArtists { get; set; }
	public required IndexedSet<SpotifyArtistReleaseEntity> ArtistsReleases { get; set; }

	public required IndexedSet<SpotifyArtistEntity> Artists { get; set; }
	public required IndexedSet<SpotifyReleaseEntity> Releases { get; set; }

	public required IndexedSet<SpotifyTrackEntity> Tracks { get; set; }

}