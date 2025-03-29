using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Managers;
using MAUSIC.ViewModels.Abstract;
using Microsoft.Maui.Controls;
using Timer = System.Timers.Timer;

namespace MAUSIC.ViewModels;

public partial class PlayerViewModel : BaseViewModel
{

    private readonly StorageManager _storageManager;

    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty] private string _songName;

    [ObservableProperty] private string _artist;

    [ObservableProperty] private string _album;

    [ObservableProperty] private ImageSource _cover = ImageSource.FromFile("dotnet_bot.png");

    [ObservableProperty] private TimeSpan _duration;

    [ObservableProperty] private double _durationTimeSliderValue;

    [ObservableProperty] private string _durationStringRepresentation;

    [ObservableProperty] private TimeSpan _currentTime;

    [ObservableProperty] private double _currentTimeSliderValue;

    [ObservableProperty] private string _currentTimeStringRepresentation;

    [ObservableProperty] private MediaSource? _currentSongPath;

    public PlayerViewModel(StorageManager storageManager)
    {
        _storageManager = storageManager;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    protected override async Task InitializeAsync()
    {
        SongName = "song";
        Artist = "artist";
        Album = "album";
        Duration = new TimeSpan(0, 4, 20);
        DurationTimeSliderValue = Duration.TotalSeconds;
        CurrentTime = TimeSpan.Zero;
        CurrentTimeSliderValue = CurrentTime.TotalSeconds;
        UpdateStringRepresentation();
    }

    [RelayCommand]
    private async Task DragCompleted()
    {
        CurrentTime = TimeSpan.FromSeconds(CurrentTimeSliderValue);
        UpdateStringRepresentation();
    }

    public async Task RequestFiles()
    {
        var files = await _storageManager.PickFolder();

        if (files == null)
        {
            return;
        }

        CurrentSongPath = MediaSource.FromFile(files[0]);

        var tagsFile = TagLib.File.Create(files[0]);
        SongName = tagsFile.Tag.Title;
        Artist = tagsFile.Tag.FirstPerformer;
        Album = tagsFile.Tag.Album;
        Duration = tagsFile.Properties.Duration;
        DurationTimeSliderValue = Duration.TotalSeconds;
        if (tagsFile.Tag.Pictures.Length > 0)
        {
            Cover = ImageSource.FromStream(() => new MemoryStream(tagsFile.Tag.Pictures[0].Data.Data));
        }
        else
        {
            Cover = ImageSource.FromFile("dotnet_bot.png");
        }

        if (CurrentTime == Duration)
        {
            CurrentTime = TimeSpan.Zero;
        }

        UpdateStringRepresentation();

        foreach (var file in files)
        {
            Console.WriteLine(file);
        }
    }

    private void UpdateStringRepresentation()
    {
        CurrentTimeStringRepresentation = CurrentTime.ToString(@"mm\:ss");

        DurationStringRepresentation = Duration.ToString(@"mm\:ss");
    }
}