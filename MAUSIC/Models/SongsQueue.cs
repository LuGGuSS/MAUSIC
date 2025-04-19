using System.Collections.ObjectModel;
using MAUSIC.Models.Abstract;

namespace MAUSIC.Models;

public class SongsQueue : BaseModel
{
    private int _currentSongIndex = -1;

    private SongModel? _currentSong;

    private List<SongModel> _songs = new();

    public Action? OnNewSongPlaying { get; set; }

    public List<SongModel> Songs
    {
        get => _songs;
        set
        {
            if (value == null)
            {
                return;
            }

            var newSongs = new List<SongModel>();

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
        };

        var songs = Songs;

        songs.Add(newSong);

        Songs = songs;

        // NotifyCollectionChanged();
    }

    public void RemoveSong(int songIndex)
    {
        var songs = Songs;

        songs.RemoveAt(songIndex);

        Songs = songs;
    }

    public void NotifyCollectionChanged()
    {
        OnPropertyChanged(nameof(Songs));
    }

    public Func<SongModel,Task> OpenPopupFunc { get; set; }
}