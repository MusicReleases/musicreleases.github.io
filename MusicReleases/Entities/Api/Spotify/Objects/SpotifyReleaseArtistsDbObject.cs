using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify.Objects;

public record SpotifyReleaseArtistsDbObject(string ReleaseId, ISet<SpotifyArtist> Artists);