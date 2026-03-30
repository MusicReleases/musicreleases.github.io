namespace JakubKastner.MusicReleases.Spotify.Releases;

internal sealed record SpotifyReleaseUrlParameters
(
	string? Type,
	string? Year,
	string? Month,
	string? ArtistId,
	string? Filter,
	string? Search
);