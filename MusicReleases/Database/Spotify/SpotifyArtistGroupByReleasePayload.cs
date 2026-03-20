using JakubKastner.MusicReleases.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal record SpotifyArtistGroupByReleasePayload(string Id, HashSet<string> MainArtistIds, HashSet<string> FeaturedArtistIds) : ISpotifyPayload;