using CommunityToolkit.Mvvm.ComponentModel;
using MAUSIC.ViewModels.Abstract;

namespace MAUSIC.ViewModels;

public partial class PlayerViewModel : BaseViewModel
{
    [ObservableProperty] private string _songName;

    [ObservableProperty] private string _artist;

    [ObservableProperty] private string _album;

    [ObservableProperty] private string _duration;

    [ObservableProperty] private string _currentTime;

    public PlayerViewModel()
    {
    }

    protected override async Task InitializeAsync()
    {
        SongName = "song";
        Artist = "artist";
        Album = "album";
        Duration = "4:20";
        CurrentTime = "00:00";
    }
}