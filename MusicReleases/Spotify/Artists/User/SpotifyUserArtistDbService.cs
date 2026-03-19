using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;

namespace JakubKastner.MusicReleases.Spotify.Artists.User;

internal sealed class SpotifyUserArtistDbService(IDbSpotifyService dbService) : SpotifyUserLinkEntityService<SpotifyUserArtistEntity, SpotifyUserArtistPayload>, ISpotifyUserArtistDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override SpotifyUserArtistPayload ToPayload(SpotifyUserArtistEntity entity) => entity.ToPayload();

	protected override SpotifyUserArtistEntity ToEntity(SpotifyUserArtistPayload payload, string userId) => payload.ToEntity(userId);

	protected override async Task<Table<SpotifyUserArtistEntity, (string, string)>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.UserArtist;
	}

	protected override async Task<IEnumerable<SpotifyUserArtistEntity>> FetchByUserId(string userId)
	{
		var table = await GetTable();
		return await table.Where(x => x.UserId, userId).ToArray();
	}
}