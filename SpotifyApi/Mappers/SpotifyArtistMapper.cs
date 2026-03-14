using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Mappers;

internal static class SpotifyArtistMapper
{
	public static SpotifyArtist ToObject(this SimpleArtist api)
	{
		return new(api.Id, api.Name, api.Uri, api.ExternalUrls[Namings.ExternalUrlSpotifyKey], true);
	}

	public static SpotifyArtist ToObject(this FullArtist api)
	{
		return new(api.Id, api.Name, api.Uri, api.ExternalUrls[Namings.ExternalUrlSpotifyKey], true);
	}

	public static SpotifyArtist ToObject(this SimpleShow api)
	{
		return new(api.Id, api.Name, api.Uri, api.ExternalUrls[Namings.ExternalUrlSpotifyKey], true);
	}

	public static SpotifyArtist ToObject(this FullShow api)
	{
		return new(api.Id, api.Name, api.Uri, api.ExternalUrls[Namings.ExternalUrlSpotifyKey], true);
	}
}
