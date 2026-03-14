using JakubKastner.SpotifyApi.Objects.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUserInfo : SpotifyIdNameUrlObject
{
	public required string? ProfilePictureUrl { get; set; }
	public DateTime LastUpdate { get; set; }

	public SpotifyUserInfo()
	{
		// ctor for json
	}

	[SetsRequiredMembers]
	public SpotifyUserInfo(string id, string name, string urlApp, string urlWeb, string? urlProfilePicture) : base(id, name, urlApp, urlWeb)
	{
		ProfilePictureUrl = urlProfilePicture;
		// TODO LastUpdate
	}

	[SetsRequiredMembers]
	public SpotifyUserInfo(string id, string name, string urlApp, string urlWeb, string? urlProfilePicture, DateTime lastUpdate) : base(id, name, urlApp, urlWeb)
	{
		ProfilePictureUrl = urlProfilePicture;
		LastUpdate = lastUpdate;
	}
}
