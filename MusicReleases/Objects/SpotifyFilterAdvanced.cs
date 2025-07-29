using JakubKastner.MusicReleases.Entities.Api.Spotify.User;

namespace JakubKastner.MusicReleases.Objects;

public class SpotifyFilterAdvanced
{
	// this names must be same as in the URL and in Enums.ReleasesFilters
	public bool Tracks { get; init; } = true;
	public bool EPs { get; init; } = true;
	public bool NotRemixes { get; init; } = true;
	public bool Remixes { get; init; } = true;
	public bool FollowedArtists { get; init; } = true;
	public bool SavedReleases { get; init; } = false;
	public bool NotVariousArtists { get; init; } = true;
	public bool VariousArtists { get; init; } = true;
	public bool NewReleases { get; init; } = true;
	public bool OldReleases { get; init; } = true;

	public SpotifyFilterAdvanced()
	{ }


	public SpotifyFilterAdvanced(bool tracks, bool eps, bool notRemixes, bool remixes, bool followedArtists, bool savedReleases, bool notVariousArtists, bool variousArtists, bool newReleases, bool oldReleases)
	{
		Tracks = tracks;
		EPs = eps;
		NotRemixes = notRemixes;
		Remixes = remixes;
		FollowedArtists = followedArtists;
		SavedReleases = savedReleases;
		NotVariousArtists = notVariousArtists;
		VariousArtists = variousArtists;
		NewReleases = newReleases;
		OldReleases = oldReleases;
	}

	public SpotifyFilterAdvanced(SpotifyFilterEntity spotifyFilterDb)
	{
		Tracks = spotifyFilterDb.Tracks;
		EPs = spotifyFilterDb.EPs;
		NotRemixes = spotifyFilterDb.NotRemixes;
		Remixes = spotifyFilterDb.Remixes;
		FollowedArtists = spotifyFilterDb.FollowedArtists;
		SavedReleases = spotifyFilterDb.SavedReleases;
		NotVariousArtists = spotifyFilterDb.NotVariousArtists;
		VariousArtists = spotifyFilterDb.VariousArtists;
		NewReleases = spotifyFilterDb.NewReleases;
		OldReleases = spotifyFilterDb.OldReleases;
	}
}
