using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Data.Entities;
using MAUSIC.Managers;
using MAUSIC.Mappers;
using MAUSIC.Models;
using MAUSIC.ViewModels.Abstract;
using Microsoft.Maui.Controls;
using Timer = System.Timers.Timer;

namespace MAUSIC.ViewModels;

public partial class PlayerViewModel : BaseViewModel
{

    private readonly StorageManager _storageManager;
    private readonly DatabaseManager _databaseManager;

    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty] private string _title;

    [ObservableProperty] private string _artist;

    [ObservableProperty] private string _album;

    [ObservableProperty] private ImageSource _cover = ImageSource.FromFile("album_100dp.png");

    [ObservableProperty] private TimeSpan _duration;

    [ObservableProperty] private double _durationTimeSliderValue;

    [ObservableProperty] private string _durationStringRepresentation;

    [ObservableProperty] private double _currentTimeSliderValue;

    [ObservableProperty] private string _currentTimeStringRepresentation;

    [ObservableProperty] private MediaSource? _currentSongPath;

    [ObservableProperty] private List<SongModel> _songs;

    [ObservableProperty] private int _currentSongIndex;

    public PlayerViewModel(
        StorageManager storageManager,
        DatabaseManager databaseManager)
    {
        _storageManager = storageManager;
        _databaseManager = databaseManager;

        _cancellationTokenSource = new CancellationTokenSource();
    }

    protected override async Task InitializeAsync()
    {
        Title = "song";
        Artist = "artist";
        Album = "album";
        Duration = new TimeSpan(0, 4, 20);
        DurationTimeSliderValue = Duration.TotalSeconds;
        CurrentTimeSliderValue = 0;
        UpdateStringRepresentation();
    }

    public async Task RequestFiles()
    {
        var files = await _storageManager.PickFolder();

        if (files == null)
        {
            return;
        }

        var songsEntities = new List<SongEntity>();

        foreach (var file in files)
        {
            var existingSong = await _databaseManager.GetItemAsync<SongEntity>((song) => song.Path == file);

            if (existingSong != null)
            {
                continue;
            }

            var tagsFile = TagLib.File.Create(file);

            var song = new SongEntity
            {
                Title = tagsFile.Tag.Title,
                Artist = tagsFile.Tag.FirstPerformer,
                Album = tagsFile.Tag.Album,
                Duration = tagsFile.Properties.Duration,
                Path = file
            };

            songsEntities.Add(song);
        }

        await _databaseManager.SaveItemsAsync(songsEntities);

        var allSongEntities = await _databaseManager.GetAllItems<SongEntity>();

        var songModels = new List<SongModel>();

        foreach (var song in allSongEntities)
        {
            var model = song.Map();

            if (model == null)
            {
                continue;
            }

            model.CoverImage = TryGetSongCover(song.Path);

            songModels.Add(model);
        }

        Songs = songModels;
        CurrentSongIndex = 0;

        EnqueueNextSong();
    }

    [RelayCommand]
    private void EnqueueNextSong()
    {
        if (CurrentSongIndex >= Songs.Count)
        {
            CurrentSongIndex = 0;
        }
        else if(CurrentSongIndex < 0)
        {
            CurrentSongIndex = Songs.Count - 1;
        }

        var currentPlayingSong = Songs.FirstOrDefault(song => song.IsPlaying);
        if (currentPlayingSong != null)
        {
            currentPlayingSong.IsPlaying = false;
        }

        var song = Songs[CurrentSongIndex];

        Title = song.Title;
        Artist = song.Artist;
        Album = song.Album;
        Duration = song.Duration;
        DurationTimeSliderValue = Duration.TotalSeconds;

        Cover = song.CoverImage;

        song.IsPlaying = true;


        CurrentSongPath = MediaSource.FromFile(Songs[CurrentSongIndex].Path);

        UpdateStringRepresentation();
    }

    [RelayCommand]
    private Task QueueSongSelected(SongModel song)
    {
        var index = Songs.IndexOf(song);

        CurrentSongIndex = index;

        EnqueueNextSong();

        return Task.CompletedTask;
    }

    private ImageSource TryGetSongCover(string songPath)
    {
        var tags = TagLib.File.Create(songPath);

        var result =tags.Tag.Pictures.Length > 0
            ? ImageSource.FromStream(() => new MemoryStream(tags.Tag.Pictures[0].Data.Data))
            : ImageSource.FromFile("album_100dp.png");

        return result;
    }

    private void UpdateStringRepresentation()
    {
        CurrentTimeStringRepresentation = TimeSpan.Zero.ToString(@"mm\:ss");

        DurationStringRepresentation = Duration.ToString(@"mm\:ss");
    }
}