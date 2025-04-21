using MAUSIC.Data.Entities;
using MAUSIC.Managers;
using MAUSIC.Models;

namespace MAUSIC.Services;

public class PlaylistService
{
    private readonly DatabaseManager _databaseManager;

    public PlaylistService(DatabaseManager databaseManager)
    {
        _databaseManager = databaseManager;
    }

    public async Task<List<PlaylistEntity>?> GetAllPlaylists()
    {
        var result = await _databaseManager.GetAllItems<PlaylistEntity>();

        return result;
    }

    public async Task<PlaylistEntity?> GetPlaylistByTitle(string title)
    {
        var result = await _databaseManager.GetItemAsync<PlaylistEntity>((entity) => entity.Title == title);

        return result;
    }

    public async Task<PlaylistEntity?> GetPlaylistById(int id)
    {
        var result = await _databaseManager.GetItemAsync<PlaylistEntity>((entity) => entity.Id == id);

        return result;
    }

    public async Task<PlaylistEntity> CreatePlaylist(string title)
    {
        var result = await _databaseManager.GetItemAsync<PlaylistEntity>((entity) => entity.Title == title);

        if (result != null)
        {
            return result;
        }

        var playlistEntity = new PlaylistEntity()
        {
            Title = title
        };

        await _databaseManager.SaveItemAsync(playlistEntity);

        return playlistEntity;
    }

    public async Task<PlaylistSongEntity> AddSong(PlaylistEntity playlistEntity, SongEntity song)
    {
        var playlistSong = await _databaseManager
            .GetItemAsync<PlaylistSongEntity>((playlistSong) =>
                playlistSong.PlaylistId == playlistEntity.Id
                && playlistSong.SongId == song.Id);

        if (playlistSong != null)
        {
            return playlistSong;
        }

        var playlistSongEntity = new PlaylistSongEntity()
        {
            PlaylistId = playlistEntity.Id,
            SongId = song.Id
        };

        await _databaseManager.SaveItemAsync(playlistSongEntity);

        return playlistSongEntity;
    }

    public async Task<bool> RemoveSong(PlaylistSongEntity playlistSongEntity)
    {
        var result = await _databaseManager.DeleteItemAsync(playlistSongEntity);

        return result;
    }

    public async Task<bool> ToggleFavourite(PlaylistEntity playlistEntity, SongEntity song)
    {
        var playlistSong = await _databaseManager.GetItemAsync<PlaylistSongEntity>((entity) => entity.SongId == song.Id && playlistEntity.Id == entity.PlaylistId);

        if (playlistSong != null)
        {
            await _databaseManager.DeleteItemAsync(playlistSong);
            return false;
        }

        playlistSong = new PlaylistSongEntity()
        {
            PlaylistId = playlistEntity.Id,
            SongId = song.Id
        };

        await _databaseManager.SaveItemAsync(playlistSong);
        return true;
    }

    public async Task<List<PlaylistSongEntity>> GetPlaylistSongs(int playlistId)
    {
        var allSongs = await _databaseManager
            .GetItemsAsync<PlaylistSongEntity>((songEntity) => songEntity.PlaylistId == playlistId);

        return allSongs;
    }
}