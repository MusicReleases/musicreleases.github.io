using JakubKastner.SpotifyApi.Artists;

namespace JakubKastner.MusicReleases.Spotify.Releases.Artists;

internal record SpotifyArtistReleasePayload(string Id, HashSet<SpotifyArtist> MainArtists, HashSet<SpotifyArtist> FeaturedArtists) : ISpotifyPayload;
