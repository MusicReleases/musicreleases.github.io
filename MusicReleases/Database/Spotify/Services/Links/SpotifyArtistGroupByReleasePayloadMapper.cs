using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Database.Spotify.Services.Links;

internal static class SpotifyArtistGroupByReleasePayloadMapper
{
	public static SpotifyArtistGroupByReleasePayload ToPayload(this IGrouping<string, ISpotifyArtistLinkEntity> group)
	{
		var mainArtistIds = new HashSet<string>();
		var featuredArtistIds = new HashSet<string>();

		foreach (var entity in group)
		{
			if (entity.Role == ArtistReleaseRole.Main)
			{
				mainArtistIds.Add(entity.ArtistId);
			}
			else if (entity.Role == ArtistReleaseRole.Featured)
			{
				featuredArtistIds.Add(entity.ArtistId);
			}
		}
		return new(group.Key, mainArtistIds, featuredArtistIds);
	}
}