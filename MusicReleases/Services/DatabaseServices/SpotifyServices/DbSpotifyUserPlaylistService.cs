﻿using JakubKastner.MusicReleases.Entities.Api.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUserPlaylistService(IDbSpotifyService dbService, IDbSpotifyPlaylistService dbPlaylistService, IDbSpotifyUpdateService dbUpdateService) : IDbSpotifyUserPlaylistService
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyUserPlaylist);

	private readonly IDbSpotifyPlaylistService _dbPlaylistService = dbPlaylistService;
	private readonly IDbSpotifyUpdateService _dbUpdateService = dbUpdateService;

	public async Task<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>?> Get(string userId)
	{
		// update
		var update = await GetUpdateDb(userId);
		if (update is null)
		{
			return null;
		}

		// artists
		var playlists = await GetSaved(userId);
		if (playlists.Count < 1)
		{
			return null;
		}

		var playlistUpdate = new SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>(playlists, update);

		return playlistUpdate;
	}

	private async Task<ISet<SpotifyPlaylist>> GetSaved(string userId)
	{
		// get followed artist ids
		var playlistsDb = await GetSavedDb(userId);
		var playlistIdsDb = playlistsDb.Select(x => x.PlaylistId);

		// get all artists (name) from db
		var userPlaylistsDb = _dbTable.GetAllAsync<SpotifyUserPlaylistEntity>();
		var playlistsDbAll = await _dbPlaylistService.GetAll();
		if (playlistsDbAll is null)
		{
			throw new NullReferenceException(nameof(playlistsDbAll));
		}

		var playlists = new HashSet<SpotifyPlaylist>();
		foreach (var playlistId in playlistIdsDb)
		{
			var playlistDb = playlistsDbAll.First(x => x.Id == playlistId);
			var playlist = new SpotifyPlaylist
			{
				Id = playlistDb.Id,
				Name = playlistDb.Name,
				CurrentUserOwned = playlistDb.CurrentUserOwned,
				Collaborative = playlistDb.Collaborative,
			};
			playlists.Add(playlist);
		}

		return playlists;
	}


	private async Task<ISet<SpotifyUserPlaylistEntity>> GetSavedDb(string userId)
	{
		// TODO user artist db table
		Console.WriteLine("get playlists");

		// get artists from db
		var userPlaylistsDb = _dbTable.GetAllAsync<SpotifyUserPlaylistEntity>();
		var playlistsDb = new HashSet<SpotifyUserPlaylistEntity>();


		await foreach (var userPlaylistDb in userPlaylistsDb)
		{
			if (userPlaylistDb.UserId != userId)
			{
				continue;
			}
			playlistsDb.Add(userPlaylistDb);
		}

		return playlistsDb;
	}

	private async Task<SpotifyUserListUpdatePlaylists?> GetUpdateDb(string userId)
	{
		var updateDb = await _dbUpdateService.Get(userId);
		var updateDbPlaylists = updateDb?.Playlists;
		if (!updateDbPlaylists.HasValue)
		{
			// TODO delete user artists
			return null;
		}

		var update = new SpotifyUserListUpdatePlaylists(updateDbPlaylists.Value);
		return update;
	}

	public async Task Save(string userId, SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists> playlists)
	{
		// TODO remove unfollowed artists and deleted releases

		if (playlists.Update is null)
		{
			// TODO
			throw new NullReferenceException(nameof(playlists.Update));
		}
		if (playlists.List is null)
		{
			// TODO
			throw new NullReferenceException(nameof(playlists.List));
		}

		// update db
		await SaveUpdateDb(userId, playlists.Update);

		// user artists db
		await SaveDb(playlists.List, userId);
	}

	private async Task SaveUpdateDb(string userId, SpotifyUserListUpdatePlaylists update)
	{
		// TODO null
		//var updateDb = await _databaseUpdateController.Get(db, userId);

		var updateDb = await _dbUpdateService.Get(userId);

		if (updateDb is null)
		{
			// TODO
			throw new NullReferenceException(nameof(updateDb));
		}

		// update - update times
		updateDb.Playlists = update.LastUpdateMain;
		await _dbUpdateService.Update(updateDb);
	}


	private async Task SaveDb(ISet<SpotifyPlaylist> playlists, string userId)
	{
		var userPlaylistsDb = await GetSavedDb(userId);

		var newPlaylists = playlists.Where(x => !userPlaylistsDb.Any(y => y.PlaylistId == x.Id)).ToHashSet();
		var deletedPlaylists = userPlaylistsDb.Where(x => !playlists.Any(y => y.Id == x.PlaylistId)).ToHashSet();

		// save new followed artists
		await _dbPlaylistService.Save(newPlaylists);
		foreach (var playlist in newPlaylists)
		{
			var playlistEntity = new SpotifyUserPlaylistEntity(userId, playlist.Id);
			await _dbTable.StoreAsync(playlistEntity);
		}

		// delete not followed artists
		await Delete(deletedPlaylists);
	}

	private async Task Delete(ISet<SpotifyUserPlaylistEntity> userPlaylistsDb)
	{
		foreach (var userPlaylistDb in userPlaylistsDb)
		{
			await _dbTable.RemoveItemAsync(userPlaylistDb);
		}
	}

	public async Task Delete(string userId)
	{
		var userPlaylistsDb = _dbTable.GetAllAsync<SpotifyUserPlaylistEntity>();
		await foreach (var userPlaylistDb in userPlaylistsDb)
		{
			if (userPlaylistDb.UserId != userId)
			{
				continue;
			}
			await _dbTable.RemoveItemAsync(userPlaylistDb);
		}
	}
}
