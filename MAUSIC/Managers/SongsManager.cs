using CommunityToolkit.Maui.Core.Extensions;
using MAUSIC.Data.Entities;
using MAUSIC.Services;

namespace MAUSIC.Managers;

public class SongsManager
{
    private readonly SongsService _songsService;
    private readonly QueueManager _queueManager;

    public SongsManager(
        SongsService songsService,
        QueueManager queueManager)
    {
        _songsService = songsService;
        _queueManager = queueManager;
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
                .ToObservableCollection();
        }
    }

    public SongEntity GetSongFromPath(string path)
    {
        var entity = _songsService.GetSongEntityFromPath(path);

        return entity;
    }
}