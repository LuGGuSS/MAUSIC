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

    public Func<SongsQueue>? GetCurrentSongsQueue { get; set; }

    public void AddSongToQueue(SongsQueue? queue, SongModel? song)
    {
        if (queue == null || song == null)
        {
            return;
        }

        _queueService.AddSongToQueue(queue, song);
    }

    public void RemoveSongFromQueue(SongsQueue? queue, SongModel? song)
    {
        if (queue == null || song == null)
        {
            return;
        }

        _queueService.RemoveSongFromQueue(queue, song);
    }

    public void ChangeSongPositionInQueue(SongsQueue? queue, int previousIndex, int newIndex)
    {
        if (queue == null)
        {
            return;
        }

        _queueService.ChangeSongPositionInQueue(queue, previousIndex, newIndex);
    }

    public void EnqueueNextSong(SongsQueue? queue)
    {
        if (queue == null)
        {
            return;
        }

        _queueService.EnqueueNextSong(queue);
    }

    public void EnqueuePreviousSong(SongsQueue? queue)
    {
        if (queue == null)
        {
            return;
        }

        _queueService.EnqueuePreviousSong(queue);
    }

    public void EnqueueSongByIndex(SongsQueue? queue, int songIndex)
    {
        if (queue == null)
        {
            return;
        }

        _queueService.EnqueueSongByIndex(queue, songIndex);
    }
}