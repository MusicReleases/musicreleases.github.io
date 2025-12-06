using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyService(IDexieNETService<SpotifyDb> dexieService) : IDbSpotifyService
{
	private const int CURRENT_DB_VERSION = 1;

	private readonly IDexieNETService<SpotifyDb> _dexieService = dexieService;
	private SpotifyDb? _dbInstance;

	private readonly SemaphoreSlim _lock = new(1, 1);

	public async ValueTask<SpotifyDb> GetDb()
	{
		if (_dbInstance is not null) return _dbInstance;

		await _lock.WaitAsync();
		try
		{
			if (_dbInstance is null)
			{
				var db = await _dexieService.DexieNETFactory.Create();

				db.Version(CURRENT_DB_VERSION).Stores();

				_dbInstance = db;
			}
			return _dbInstance;
		}
		finally
		{
			_lock.Release();
		}
	}
}
