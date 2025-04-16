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

        playlistEntity.Id = await _databaseManager.SaveItemAsync(playlistEntity);

        return playlistEntity;
    }

    public async Task<PlaylistSongEntity> AddSong(PlaylistEntity playlistEntity, SongModel song)
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

        playlistEntity.Id = await _databaseManager.SaveItemAsync(playlistSongEntity);

        return playlistSongEntity;
    }

    public async Task<List<PlaylistSongEntity>> GetPlaylistSongs(PlaylistEntity playlistEntity)
    {
        var allSongs = await _databaseManager
            .GetItemsAsync<PlaylistSongEntity>((songEntity) => songEntity.PlaylistId == playlistEntity.Id);

        return allSongs;
    }
}