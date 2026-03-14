using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.SpotifyApi.Objects;

[method: SetsRequiredMembers]
public class SpotifyUser(SpotifyUserInfo info, SpotifyUserCredentials credentials)
{
	public SpotifyUserInfo Info { get; internal set; } = info;

	public SpotifyUserCredentials Credentials { get; internal set; } = credentials;
}

internal static class SpotifyUserExtensions
{
	public static void ThrowIfNull([NotNull] this SpotifyUser? value)
	{
		if (value is null)
		{
			throw new UnauthorizedAccessException(nameof(SpotifyUser));
		}
	}
}