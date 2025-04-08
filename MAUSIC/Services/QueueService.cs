using MAUSIC.Models;

namespace MAUSIC.Services;

public class QueueService
{
    public void RemoveSongFromQueue(SongsQueue? queue, SongModel? song)
    {
        if (queue?.Songs == null || queue.Songs.Count == 0 || song == null)
        {
            return;
        }

        var index = queue.Songs.IndexOf(song);
        queue.Songs.Remove(song);

        if (queue.CurrentSongIndex > index)
        {
            queue.CurrentSongIndex--;
        }
        else if(queue.CurrentSongIndex == index)
        {
            // NOTE: this will call underlying logic to set proper index and current song
            queue.CurrentSongIndex = index;
        }
    }

    public void ChangeSongPositionInQueue(SongsQueue? queue, int previousIndex, int newIndex)
    {
        if (queue?.Songs == null || queue.Songs.Count == 0)
        {
            return;
        }

        var song = queue.Songs[previousIndex];
        queue.Songs.RemoveAt(previousIndex);
        queue.Songs.Insert(newIndex, song);

        if (queue.CurrentSongIndex > newIndex && queue.CurrentSongIndex < previousIndex)
        {
            queue.CurrentSongIndex++;
        }
        else if (queue.CurrentSongIndex < newIndex && queue.CurrentSongIndex > previousIndex)
        {
            queue.CurrentSongIndex--;
        }
    }

    public void EnqueueNextSong(SongsQueue? queue)
    {
        if (queue?.Songs == null || queue.Songs.Count == 0)
        {
            return;
        }

        queue.CurrentSongIndex++;
    }

    public void EnqueuePreviousSong(SongsQueue? queue)
    {
        if (queue?.Songs == null || queue.Songs.Count == 0)
        {
            return;
        }

        queue.CurrentSongIndex--;
    }

    public void EnqueueSongByIndex(SongsQueue? queue, int songIndex)
    {
        if (queue?.Songs == null || queue.Songs.Count == 0)
        {
            return;
        }

        queue.CurrentSongIndex = songIndex;
    }
}