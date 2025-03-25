using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public interface IDatabaseUpdateController
{
	Task Delete(string userId);
	Task<SpotifyLastUpdateEntity?> Get(IndexedDb db, string userId);
	Task<SpotifyLastUpdateEntity> GetOrCreate(IndexedDb db, string userId);
}