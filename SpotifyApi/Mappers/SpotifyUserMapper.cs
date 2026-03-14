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

		return new SpotifyUserInfo(api.Id, api.DisplayName, api.Uri, api.ExternalUrls[Namings.ExternalUrlSpotifyKey], urlProfilePicture, lastUpdate);
	}

	private static string? GetImageUrl(List<Image> images)
	{
		if (images.Count < Namings.SmallImageIndex)
		{
			return null;
		}

		if (images.Count >= Namings.MediumImageIndex)
		{
			return images[Namings.MediumImageIndex].Url;
		}

		return images[Namings.SmallImageIndex].Url;
	}
}
