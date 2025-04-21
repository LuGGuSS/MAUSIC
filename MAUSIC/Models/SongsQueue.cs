using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using CommunityToolkit.Maui.Core.Extensions;
using MAUSIC.Models.Abstract;

namespace MAUSIC.Models;

public class SongsQueue : BaseModel, INotifyCollectionChanged
{
    private readonly Random _random = new();

    private int _currentSongIndex = -1;

    private SongModel? _currentSong;

    private ObservableCollection<SongModel> _songs = new();
    private ObservableCollection<SongModel> _originalQueue;

    public Action? OnNewSongPlaying { get; set; }

    public ObservableCollection<SongModel> Songs
    {
        get => _songs;
        set
        {
            if (value == null)
            {
                return;
            }

            var newSongs = new ObservableCollection<SongModel>();

            foreach (var song in value)
            {
                var newSong = new SongModel
                {
                    Album = song.Album,
                    Artist = song.Artist,
                    CoverImage = song.CoverImage,
                    Duration = song.Duration,
                    Id = song.Id,
                    IsPlaying = song.IsPlaying,
                    OpenPopupFunc = OpenPopupFunc,
                    Path = song.Path,
                    Title = song.Title,
                    IsFavorite = song.IsFavorite,
                };

                newSongs.Add(newSong);
            }

            if (SetField(ref _songs, newSongs))
            {
                CurrentSongIndex = 0;
            }
        }
    }

    public SongModel? CurrentSong
    {
        get => _currentSong;
        set => SetField(ref _currentSong, value);
    }

    public int CurrentSongIndex
    {
        get => _currentSongIndex;
        set
        {
            if (value >= Songs!.Count)
            {
                value = 0;
            }
            else if(value < 0)
            {
                value = Songs.Count - 1;
            }

            if(SetField(ref _currentSongIndex, value))
            {
                if (CurrentSong != null)
                {
                    CurrentSong.IsPlaying = false;
                }

                CurrentSong = Songs[CurrentSongIndex];
                CurrentSong.IsPlaying = true;

                OnNewSongPlaying?.Invoke();
            }
        }
    }

    public void ToggleShuffle(bool shuffle)
    {
        if (Songs.Count <= 1)
        {
            return;
        }

        if (shuffle)
        {
            _originalQueue = Songs;

            var song = CurrentSong;

            var newQueue = Songs.OrderBy(entity => _random.Next()).ToObservableCollection();

            newQueue.Remove(song!);
            newQueue.Insert(0, song!);

            _songs = newQueue;
            _currentSongIndex = 0;
        }
        else
        {
            var index = _originalQueue.IndexOf(CurrentSong!);

            _songs = _originalQueue;
            _currentSongIndex = index;
        }

        OnPropertyChanged(nameof(Songs));
        OnPropertyChanged(nameof(CurrentSongIndex));
    }

    public void AddSong(SongModel song)
    {
        var newSong = new SongModel
        {
            Album = song.Album,
            Artist = song.Artist,
            CoverImage = song.CoverImage,
            Duration = song.Duration,
            Id = song.Id,
            IsPlaying = song.IsPlaying,
            OpenPopupFunc = OpenPopupFunc,
            Path = song.Path,
            Title = song.Title,
            IsFavorite = song.IsFavorite,
            IsRecommended = song.IsRecommended
        };

        Songs.Add(newSong);
    }

    public void RemoveSong(int songIndex)
    {
        Songs.RemoveAt(songIndex);
    }

    public void InsertSong(int index, SongModel song)
    {
        Songs.Insert(index, song);
    }

    protected virtual void OnCollectionChanged(NotifyCollectionChangedAction action)
    {
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
    }

    public Func<SongModel,Task> OpenPopupFunc { get; set; }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
}