using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Store
{
	public interface ISpotifyUserStore
	{
		string GetRefreshTokenRequired();
		SpotifyUser? GetUser();
		string GetUserIdRequired();
		SpotifyUser GetUserRequired();
		void SetRefreshToken(string refreshToken);
		void SetUser(PrivateUser userApi, string refreshToken);
		void SetUser(SpotifyUser user);
		void DeleteUser();
		void SetUserInfoApi(PrivateUser userApi);
		bool UserIsNotNull();
	}
}