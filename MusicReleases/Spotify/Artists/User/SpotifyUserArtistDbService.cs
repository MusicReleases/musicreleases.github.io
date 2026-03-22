using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Links;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Spotify.Artists.User;

internal sealed class SpotifyUserArtistDbService(IDbSpotifyService dbService) : SpotifyUserLinkEntityService<SpotifyUserArtistEntity, SpotifyUserArtistPayload>, ISpotifyUserArtistDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override Expression<Func<SpotifyUserArtistEntity, string>> Key1Expression => x => x.UserId;

	protected override Expression<Func<SpotifyUserArtistEntity, string>> Key2Expression => x => x.ArtistId;

	protected override SpotifyUserArtistEntity ToEntityFromKey1(SpotifyUserArtistPayload payload, string key1) => payload.ToEntity(key1);

	protected override SpotifyUserArtistPayload ToPayload1(SpotifyUserArtistEntity entity) => entity.ToPayload();

	protected override async Task<Table<SpotifyUserArtistEntity, (string, string)>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.UserArtist;
	}
}