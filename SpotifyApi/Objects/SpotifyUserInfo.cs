using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserInfo
{
	public required string Id { get; set; }
	public required string Name { get; set; }
	public required string Country { get; set; }
	public required string? ProfilePictureUrl { get; set; }
	public required DateTime LastUpdate { get; set; }

	public SpotifyUserInfo() { }

	[SetsRequiredMembers]
	public SpotifyUserInfo(PrivateUser userApi)
	{
		Id = userApi.Id;
		Name = userApi.DisplayName;
		Country = userApi.Country;
		ProfilePictureUrl = userApi.Images?.FirstOrDefault()?.Url;
		LastUpdate = DateTime.Now;
	}
}
