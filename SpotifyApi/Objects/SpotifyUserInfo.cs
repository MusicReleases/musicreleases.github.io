using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserInfo
{
	public string? Id { get; set; }
	public string? Name { get; set; }
	public string? Country { get; set; }
	public Image? ProfilePicture { get; set; }
	public DateTime LastUpdate { get; set; }

	public SpotifyUserInfo() { }

	public SpotifyUserInfo(PrivateUser userApi)
	{
		Id = userApi.Id;
		Name = userApi.DisplayName;
		Country = userApi.Country;
		ProfilePicture = userApi.Images?.FirstOrDefault();
		LastUpdate = DateTime.Now;
	}
}
