using CommunityToolkit.Maui.Core.Extensions;
using MAUSIC.Data.Entities;
using MAUSIC.Services;

namespace MAUSIC.Managers;

public class SongsManager
{
    private readonly SongsService _songsService;
    private readonly QueueManager _queueManager;
    private readonly DatabaseManager _databaseManager;

    public SongsManager(
        SongsService songsService,
        QueueManager queueManager,
        DatabaseManager databaseManager)
    {
        _songsService = songsService;
        _queueManager = queueManager;
        _databaseManager = databaseManager;
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
        var entity = await _databaseManager.GetItemAsync<SongEntity>((entity) => entity.Path == path);

        if (entity == null)
        {
            entity = _songsService.GetSongEntityFromPath(path);

            await _databaseManager.SaveItemAsync(entity);
        }

        return entity;
    }
}