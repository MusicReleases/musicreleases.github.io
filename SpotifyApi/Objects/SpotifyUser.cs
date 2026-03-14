using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyUser
{
	public required SpotifyUserInfo Info { get; set; }
	public required SpotifyUserCredentials Credentials { get; set; }

	public SpotifyUser()
	{
		// for di registration
	}


	[SetsRequiredMembers]
	public SpotifyUser(SpotifyUserInfo info, SpotifyUserCredentials credentials)
	{
		Info = info;
		Credentials = credentials;
	}
}

public static class SpotifyUserExtensions
{
	public static void ThrowIfNull([NotNull] this SpotifyUser? value)
	{
		if (value is null)
		{
			throw new UnauthorizedAccessException(nameof(SpotifyUser));
		}
	}
}