using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Mappers;

internal static class SpotifyUserMapper
{
	public static SpotifyUser ToObject(this PrivateUser api, string refreshToken)
	{
		var info = api.ToObjectInfo();
		var credentials = new SpotifyUserCredentials(refreshToken);

		return new(info, credentials);
	}

	public static SpotifyUserInfo ToObjectInfo(this PrivateUser api)
	{
		var urlProfilePicture = GetImageUrl(api.Images);
		var lastUpdate = DateTime.Now;

		return new SpotifyUserInfo(api.Id, api.DisplayName, api.Uri, api.ExternalUrls[ApiConventions.ExternalUrlSpotifyKey], urlProfilePicture, lastUpdate);
	}

	private static string? GetImageUrl(List<Image> images)
	{
		if (images.Count < ApiConventions.SmallImageIndex)
		{
			return null;
		}

		if (images.Count >= ApiConventions.MediumImageIndex)
		{
			return images[ApiConventions.MediumImageIndex].Url;
		}

		return images[ApiConventions.SmallImageIndex].Url;
	}
}
