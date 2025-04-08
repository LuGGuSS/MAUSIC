using MAUSIC.Models;
using MAUSIC.Services;

namespace MAUSIC.Managers;

public class QueueManager
{
    private readonly QueueService _queueService;

    public QueueManager(QueueService queueService)
    {
        _queueService = queueService;
    }

    public IList<SongModel>? GetAllSongs()
    {
        var songs = _queueService.GetAllSongs();

        return songs;
    }

    public SongModel? GetCurrentSong()
    {
        var song = _queueService.GetCurrentSong();

        return song;
    }

    public SongModel? InitQueue(IList<SongModel> queue)
    {
        var song = _queueService.InitQueue(queue);

        return song;
    }

    public IList<SongModel>? AddSongToQueue(SongModel song)
    {
        var songs = _queueService.AddSongToQueue(song);

        return songs;
    }

    public IList<SongModel>? RemoveSongFromQueue(SongModel song)
    {
        var songs = _queueService.RemoveSongFromQueue(song);

        return songs;
    }

    public IList<SongModel>? ChangeSongPositionInQueue(int previousIndex, int newIndex)
    {
        var songs = _queueService.ChangeSongPositionInQueue(previousIndex, newIndex);

        return songs;
    }

    public SongModel? EnqueueNextSong()
    {
        var song = _queueService.EnqueueNextSong();

        return song;
    }

    public SongModel? EnqueuePreviousSong()
    {
        var song = _queueService.EnqueuePreviousSong();

        return song;
    }

    public SongModel? EnqueueSongByIndex(int songIndex)
    {
        var song = _queueService.EnqueueSongByIndex(songIndex);

        return song;
    }
}