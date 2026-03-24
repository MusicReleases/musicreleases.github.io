namespace JakubKastner.MusicReleases.Database.Spotify.Ids;
/*
internal abstract class SpotifyUserIdEntityService<TModel, TUserIdEntity>(ISpotifyUserClient spotifyUserClient) : SpotifyIdEntityService<TModel, TUserIdEntity>
	where TModel : class
	where TUserIdEntity : ISpotifyDb
{
	private readonly ISpotifyUserClient _spotifyUserClient = spotifyUserClient;

	protected override string GetModelId(TModel model)
	{
		return GetUserId();
	}

	public string GetUserId()
	{
		return _spotifyUserClient.GetUserIdRequired();
	}
}*/