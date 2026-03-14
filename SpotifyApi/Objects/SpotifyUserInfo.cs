using JakubKastner.SpotifyApi.Objects.Base;
using SpotifyAPI.Web;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserInfo : SpotifyIdNameUrlObject
{
	public required string? ProfilePictureUrl { get; set; }
	public DateTime LastUpdate { get; set; }

	public SpotifyUserInfo() { }

	[SetsRequiredMembers]
	public SpotifyUserInfo(string id, string name, string urlApp, string urlWeb, string? urlProfilePicture)
	{
		Id = id;
		Name = name;
		UrlApp = urlApp;
		UrlWeb = urlWeb;
		ProfilePictureUrl = urlProfilePicture;
		// TODO LastUpdate
	}

	[SetsRequiredMembers]
	public SpotifyUserInfo(PrivateUser userApi)
	{
		Id = userApi.Id;
		Name = userApi.DisplayName;
		UrlApp = userApi.Uri;
		UrlWeb = userApi.ExternalUrls["spotify"];
		ProfilePictureUrl = userApi.Images?.LastOrDefault()?.Url;
		LastUpdate = DateTime.Now;
	}
}
