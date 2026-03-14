using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Mappers;

internal static class SpotifyTrackMapper
{
	public static SpotifyTrack ToObject(this SimpleTrack api, SpotifyRelease release)
	{
		var duration = TimeSpan.FromMilliseconds(api.DurationMs);
		var artists = GetArtists(api.Artists, release.Artists);

		return new(api.Id, api.Name, api.Uri, api.ExternalUrls[Namings.ExternalUrlSpotifyKey], release.Id, api.TrackNumber, api.DiscNumber, duration, api.Explicit, artists);
	}

	public static SpotifyTrack ToObject(this FullTrack api, SpotifyRelease release)
	{
		var duration = TimeSpan.FromMilliseconds(api.DurationMs);
		var artists = GetArtists(api.Artists, release.Artists);
		// TODO artists from api?
		//var artistsApi = api.Artists.Select(x => x.ToObject()).ToList();

		return new(api.Id, api.Name, api.Uri, api.ExternalUrls[Namings.ExternalUrlSpotifyKey], release.Id, api.TrackNumber, api.DiscNumber, duration, api.Explicit, artists);
	}

	private static HashSet<SpotifyArtist> GetArtists(List<SimpleArtist> artistsApi, HashSet<SpotifyArtist> releaseArtists)
	{
		var artists = new HashSet<SpotifyArtist>();
		foreach (var simpleArtist in artistsApi)
		{
			if (releaseArtists.Any(x => x.Id == simpleArtist.Id))
			{
				// skip for artists from album
				continue;
			}
			var artist = simpleArtist.ToObject();
			artists.Add(artist);
		}
		return artists;
	}
}
