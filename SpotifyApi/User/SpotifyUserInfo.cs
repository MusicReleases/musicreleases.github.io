using JakubKastner.SpotifyApi.Base.Objects;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.User;

public class SpotifyUserInfo : SpotifyIdNameUrlObject
{
	public required string? ProfilePictureUrl { get; set; }
	public DateTime LastUpdate { get; set; }

	[SetsRequiredMembers]
	public SpotifyUserInfo(string id, string name, string urlApp, string urlWeb, string? urlProfilePicture, DateTime lastUpdate) : base(id, name, urlApp, urlWeb)
	{
		ProfilePictureUrl = urlProfilePicture;
		LastUpdate = lastUpdate;
	}
}
