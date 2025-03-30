using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using MAUSIC.Models.Abstract;

namespace MAUSIC.Models;

public class SongModel : BaseModel
{
    private bool _isPlaying;

    public string Title { get; set; }

    public string Artist { get; set; }

    public string Album { get; set; }

    public TimeSpan Duration { get; set; }

    public string DurationString => Duration.ToString(@"mm\:ss");

    public string Path { get; set; }

    public ImageSource CoverImage { get; set; }

    public bool IsPlaying
    {
        get => _isPlaying;
        set
        {
            _isPlaying = value;

            OnPropertyChanged();
        }
    }
}