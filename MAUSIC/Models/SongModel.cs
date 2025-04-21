using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using MAUSIC.Models.Abstract;

namespace MAUSIC.Models;

public class SongModel : BaseModel
{
    private bool _isPlaying;
    private bool _isFavorite;
    private bool _isRecommended;

    public int Id { get; set; }

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
        set => SetField(ref _isPlaying, value);
    }

    public bool IsFavorite
    {
        get => _isFavorite;
        set => SetField(ref _isFavorite, value);
    }

    public bool IsRecommended
    {
        get => _isRecommended;
        set => SetField(ref _isRecommended, value);
    }

    public string Genres { get; set; }

    public string Performers { get; set; }

    public uint BPM { get; set; }

    public Func<SongModel, Task> OpenPopupFunc { get; set; }

    public async Task OpenMoreAsync()
    {
        var task = OpenPopupFunc(this);

        await task.ConfigureAwait(false);
    }
}