namespace JakubKastner.MusicReleases.Objects;

public class SpotifyFilterAdvanced
{
	// this names must be same as in the URL and in Enums.ReleasesFilters
	public bool Tracks { get; init; } = true;
	public bool EPs { get; init; } = true;
	public bool Remixes { get; init; } = false;
	public bool FollowedArtists { get; init; } = true;
	public bool VariousArtists { get; init; } = false;
	public bool InLibrary { get; init; } = false;
	public bool OnlyNew { get; init; } = false;

	public SpotifyFilterAdvanced()
	{ }


	public SpotifyFilterAdvanced(bool tracks, bool eps, bool remixes, bool followedArtists, bool variousArtists, bool library, bool newReleases)
	{
		Tracks = tracks;
		EPs = eps;
		Remixes = remixes;
		FollowedArtists = followedArtists;
		VariousArtists = variousArtists;
		InLibrary = library;
		OnlyNew = newReleases;
	}
}
