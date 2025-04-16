using System.Collections.ObjectModel;
using MAUSIC.Models.Abstract;

namespace MAUSIC.Models;

public class SongsQueue : BaseModel
{
    private int _currentSongIndex = -1;

    private SongModel? _currentSong;

    private ObservableCollection<SongModel>? _songs;

    public Action? OnNewSongPlaying { get; set; }

    public ObservableCollection<SongModel>? Songs
    {
        get => _songs;
        set
        {
            if (SetField(ref _songs, value))
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
}