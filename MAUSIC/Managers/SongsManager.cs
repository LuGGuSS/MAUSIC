using CommunityToolkit.Maui.Core.Extensions;
using MAUSIC.Data.Entities;
using MAUSIC.Services;

namespace MAUSIC.Managers;

public class SongsManager
{
    private readonly SongsService _songsService;
    private readonly QueueManager _queueManager;
    private readonly DatabaseManager _databaseManager;
    private readonly PlaylistManager _playlistManager;

    public SongsManager(
        SongsService songsService,
        QueueManager queueManager,
        DatabaseManager databaseManager,
        PlaylistManager playlistManager)
    {
        _songsService = songsService;
        _queueManager = queueManager;
        _databaseManager = databaseManager;
        _playlistManager = playlistManager;
    }

    public async Task LoadSongsFromFilesAsync(IList<string> files)
    {
        if (_queueManager.GetCurrentSongsQueue == null)
        {
            return;
        }

        await _songsService.TrySaveSongs(files);

        var songModels = await _songsService.GetAllSongModels();

        if (songModels != null)
        {
            _queueManager.GetCurrentSongsQueue.Invoke().Songs = songModels
                .OrderBy(songModel => songModel.Path)
                .ToList();
        }
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
}