using MAUSIC.Models.Abstract;

namespace MAUSIC.Models;

public class SongsQueue : BaseModel
{
    private int _currentSongIndex = -1;

    private SongModel? _currentSong;

    private IList<SongModel>? _songs;

    public IList<SongModel>? Songs
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
                var currentPlayingSong = Songs.FirstOrDefault(song => song.IsPlaying);
                if (currentPlayingSong != null)
                {
                    currentPlayingSong.IsPlaying = false;
                }

                CurrentSong = Songs[CurrentSongIndex];
            }
        }
    }
}