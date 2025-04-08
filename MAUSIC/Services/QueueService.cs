using MAUSIC.Models;

namespace MAUSIC.Services;

public class QueueService
{
    private IList<SongModel>? _queue;
    private SongModel? _currentSong;
    private int _currentSongIndex;

    public IList<SongModel>? GetAllSongs()
    {
        return _queue;
    }

    public SongModel? GetCurrentSong()
    {
        return _currentSong;
    }

    public SongModel? InitQueue(IList<SongModel> queue)
    {
        _queue = queue;
        _currentSongIndex = 0;
        _currentSong = _queue[_currentSongIndex];

        return HandleIndexChanged();
    }

    public IList<SongModel>? AddSongToQueue(SongModel song)
    {
        _queue?.Add(song);

        return _queue;
    }

    public IList<SongModel>? RemoveSongFromQueue(SongModel song)
    {
        var index = _queue?.IndexOf(song);
        _queue?.Remove(song);

        if (_currentSongIndex > index)
        {
            _currentSongIndex--;
        }
        else if(_currentSongIndex == index)
        {
            HandleIndexChanged();
        }

        return _queue;
    }

    public IList<SongModel>? ChangeSongPositionInQueue(int previousIndex, int newIndex)
    {
        if (_queue == null)
        {
            return null;
        }

        var song = _queue[previousIndex];
        _queue.RemoveAt(previousIndex);
        _queue.Insert(newIndex, song);

        if (_currentSongIndex > newIndex && _currentSongIndex < previousIndex)
        {
            _currentSongIndex++;
        }
        else if (_currentSongIndex < newIndex && _currentSongIndex > previousIndex)
        {
            _currentSongIndex--;
        }

        return _queue;
    }

    public SongModel? EnqueueNextSong()
    {
        if (_queue == null)
        {
            return null;
        }

        _currentSongIndex++;

        return HandleIndexChanged();
    }

    public SongModel? EnqueuePreviousSong()
    {
        if (_queue == null)
        {
            return null;
        }

        _currentSongIndex--;

        return HandleIndexChanged();
    }

    public SongModel? EnqueueSongByIndex(int songIndex)
    {
        if (_queue == null)
        {
            return null;
        }

        _currentSongIndex = songIndex;

        return HandleIndexChanged();
    }

    private SongModel? HandleIndexChanged()
    {
        if (_currentSongIndex >= _queue!.Count)
        {
            _currentSongIndex = 0;
        }
        else if(_currentSongIndex < 0)
        {
            _currentSongIndex = _queue.Count - 1;
        }

        var currentPlayingSong = _queue.FirstOrDefault(song => song.IsPlaying);
        if (currentPlayingSong != null)
        {
            currentPlayingSong.IsPlaying = false;
        }

        _currentSong = _queue[_currentSongIndex];

        return _currentSong;
    }
}