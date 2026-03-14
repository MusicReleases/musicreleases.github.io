using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Services.Api
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