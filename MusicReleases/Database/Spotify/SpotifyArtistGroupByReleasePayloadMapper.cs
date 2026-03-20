using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal static class SpotifyArtistGroupByReleasePayloadMapper
{
	/*public static ISpotifyArtistLinkEntity ToEntity(this SpotifyArtistGroupByReleasePayload payload, string userId)
	{
		return new(userId, payload.Id, payload.Order);
	}*/
	public static SpotifyArtistGroupByReleasePayload ToPayload(this IGrouping<string, ISpotifyArtistLinkEntity> group)
	{
		HashSet<string> mainArtistIds = [];
		HashSet<string> featuredArtistIds = [];

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
	/*public static SpotifyArtistGroupByReleasePayload ToPayload(this SpotifyPlaylist model)
	{
		return new(model.Id, model.Order);
	}*/
}