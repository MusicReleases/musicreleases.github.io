using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Store;

internal class SpotifyUserStore : ISpotifyUserStore
{
	private SpotifyUser? _user;

	public bool UserIsNotNull()
	{
		return _user is not null;
	}

	public void SetUser(SpotifyUser user)
	{
		_user = user;
	}

	public void SetUser(PrivateUser userApi, string refreshToken)
	{
		_user = userApi.ToObject(refreshToken);
	}

	public void SetUserInfoApi(PrivateUser userApi)
	{
		_user.ThrowIfNull();

		_user.Info = userApi.ToObjectInfo();
	}

	public void SetRefreshToken(string refreshToken)
	{
		_user.ThrowIfNull();

		_user.Credentials = _user.Credentials with
		{
			RefreshToken = refreshToken,
		};
	}

	public SpotifyUser? GetUser()
	{
		return _user;
	}

	public SpotifyUser GetUserRequired()
	{
		_user.ThrowIfNull();

		return _user;
	}

	public string GetUserIdRequired()
	{
		_user.ThrowIfNull();

		return _user.Info.Id;
	}

	public string GetRefreshTokenRequired()
	{
		_user.ThrowIfNull();

		return _user.Credentials.RefreshToken;
	}

	public void DeleteUser()
	{
		_user = null;
	}
}
