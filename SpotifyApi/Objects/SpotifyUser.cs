using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

[method: SetsRequiredMembers]
public class SpotifyUser(SpotifyUserInfo info, SpotifyUserCredentials credentials)
{
	public required SpotifyUserInfo Info { get; set; } = info;

	public required SpotifyUserCredentials Credentials { get; set; } = credentials;
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