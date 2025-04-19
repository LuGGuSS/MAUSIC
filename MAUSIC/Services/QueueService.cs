using MAUSIC.Models;

namespace MAUSIC.Services;

public class QueueService
{
    public void AddSongToQueue(SongsQueue queue, SongModel song)
    {
        var model = new SongModel()
        {
            Album = song.Album,
            Artist = song.Artist,
            Title = song.Title,
            CoverImage = song.CoverImage,
            Duration = song.Duration,
            Id = song.Id,
            IsPlaying = song.IsPlaying,
            OpenPopupFunc = queue.OpenPopupFunc,
            Path = song.Path,
            IsFavorite = song.IsFavorite
        };

        queue.AddSong(model);
    }

    public void RemoveSongFromQueue(SongsQueue queue, SongModel song)
    {
        if (queue.Songs.Count == 0)
        {
            return;
        }

        var index = queue.Songs.FindIndex((songModel) => songModel.Id == song.Id);

        if (index == -1)
        {
            return;
        }

        queue.RemoveSong(index);

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
        queue.NotifyCollectionChanged();

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