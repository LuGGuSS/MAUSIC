using CommunityToolkit.Maui.Core.Extensions;
using MAUSIC.Data.Entities;
using MAUSIC.Services;

namespace MAUSIC.Managers;

public class SongsManager
{
    private readonly SongsService _songsService;
    private readonly DatabaseManager _databaseManager;
    private readonly PlaylistManager _playlistManager;

    public SongsManager(
        SongsService songsService,
        DatabaseManager databaseManager,
        PlaylistManager playlistManager)
    {
        _songsService = songsService;
        _databaseManager = databaseManager;
        _playlistManager = playlistManager;
    }

    public async Task<SongEntity> GetSongFromPath(string path)
    {
        var entity = await _songsService.GetSongEntityFromPath(path);

        if (entity == null)
        {
            entity = _songsService.ReadSongEntityFromPath(path);

            await _databaseManager.SaveItemAsync(entity);

            await _playlistManager.AddSongToAllSongsPlaylist(entity);
        }

        return entity;
    }

    public async Task<SongEntity?> GetSongFromId(int id)
    {
        var entity = await _songsService.GetSongEntityFromId(id);

        return entity;
    }

    public async Task<bool> SaveSong(SongEntity song)
    {
        var result = await _songsService.SaveSong(song);

        return result;
    }

    public async Task<List<SongEntity>> GetAllSongs()
    {
        var result = await _songsService.GetAllSongs();

        return result;
    }
}