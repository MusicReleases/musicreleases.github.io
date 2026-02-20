using JakubKastner.SpotifyApi.Objects.Base;
using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserInfo : SpotifyIdNameUrlObject
{
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
		UrlApp = userApi.Uri;
		UrlWeb = userApi.Href;
		ProfilePictureUrl = userApi.Images?.LastOrDefault()?.Url;
		LastUpdate = DateTime.Now;
	}
}
