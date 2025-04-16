using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Managers;
using MAUSIC.Models;
using MAUSIC.PageModels.Abstract;

namespace MAUSIC.PageModels;

public partial class QueuePageModel : BasePageModel
{
    private readonly StorageManager _storageManager;
    private readonly SongsManager _songsManager;
    private readonly QueueManager _queueManager;

    [ObservableProperty] private SongsQueue _queue;

    public QueuePageModel(
        StorageManager storageManager,
        SongsManager songsManager,
        QueueManager queueManager)
    {
        _storageManager = storageManager;
        _songsManager = songsManager;
        _queueManager = queueManager;
    }

    protected override Task InitializeAsync()
    {
        Queue = _queueManager.GetCurrentSongsQueue?.Invoke() ?? new SongsQueue();

        return Task.CompletedTask;
    }

    public async Task RequestFiles()
    {
        var files = await _storageManager.PickFolder();

        if (files == null || files.Count == 0)
        {
            return;
        }

        await _songsManager.LoadSongsFromFilesAsync(files);
    }

    [RelayCommand]
    private void QueueSongSelected(SongModel song)
    {
        if (Queue.Songs == null)
        {
            return;
        }

        var index = Queue.Songs.IndexOf(song);

        _queueManager.EnqueueSongByIndex(Queue, index);
    }
}