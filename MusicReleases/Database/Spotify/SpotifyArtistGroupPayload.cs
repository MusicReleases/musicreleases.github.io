namespace JakubKastner.MusicReleases.Database.Spotify;

internal record SpotifyArtistGroupPayload(string ReleaseId, HashSet<string> MainArtistIds, HashSet<string> FeaturedArtistIds);