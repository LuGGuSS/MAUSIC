using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using MAUSIC.Models.Abstract;

namespace MAUSIC.Models;

public class SongsQueue : BaseModel, INotifyCollectionChanged
{
    private int _currentSongIndex = -1;

    private SongModel? _currentSong;

    private ObservableCollection<SongModel> _songs = new();

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

        // NotifyCollectionChanged();
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