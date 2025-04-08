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
    private readonly QueueManager _queueManager;

    private readonly SemaphoreSlim _semaphore = new(1);

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

    public PlayerViewModel(
        StorageManager storageManager,
        DatabaseManager databaseManager,
        QueueManager queueManager)
    {
        _storageManager = storageManager;
        _databaseManager = databaseManager;
        _queueManager = queueManager;

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

        var dbCheckTasks = new List<Task>();

        foreach (var file in files)
        {
            var existingSong = await _databaseManager.GetItemAsync<SongEntity>((song) => song.Path == file);

            if (existingSong != null)
            {
                continue;
            }

            var task = new Task(() =>
            {
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
            });

            dbCheckTasks.Add(task);
        }

        await Task.WhenAll(dbCheckTasks);

        await _databaseManager.SaveItemsAsync(songsEntities);

        var allSongEntities = await _databaseManager.GetAllItems<SongEntity>();

        var songModels = new List<SongModel>();

        var initSongEntityTasks = new List<Task>();

        foreach (var song in allSongEntities)
        {
            var task = Task.Run(() =>
            {
                var model = song.Map();

                if (model == null)
                {
                    return;
                }

                model.CoverImage = TryGetSongCover(song.Path);

                _semaphore.Wait();
                songModels.Add(model);
                _semaphore.Release();
            });

            initSongEntityTasks.Add(task);
        }

        await Task.WhenAll(initSongEntityTasks);

        Songs = songModels.OrderBy(songModel => songModel.Path).ToList();

        var currentSong = _queueManager.InitQueue(Songs);

        if (currentSong == null)
        {
            return;
        }

        EnqueueNextSong();
    }

    [RelayCommand]
    private void EnqueueNextSong()
    {
        var nextSong = _queueManager.EnqueueNextSong();

        if (nextSong == null)
        {
            return;
        }

        OnNewSongPlaying(nextSong);
    }

    [RelayCommand]
    private void EnqueuePreviousSong()
    {
        var song = _queueManager.EnqueuePreviousSong();

        if (song == null)
        {
            return;
        }

        OnNewSongPlaying(song);
    }

    [RelayCommand]
    private void QueueSongSelected(SongModel song)
    {
        var index = Songs.IndexOf(song);

        var newSong = _queueManager.EnqueueSongByIndex(index);

        if (newSong == null)
        {
            return;
        }

        OnNewSongPlaying(newSong);
    }

    private ImageSource TryGetSongCover(string songPath)
    {
        var tags = TagLib.File.Create(songPath);

        var result =tags.Tag.Pictures.Length > 0
            ? ImageSource.FromStream(() => new MemoryStream(tags.Tag.Pictures[0].Data.Data))
            : ImageSource.FromFile("album_100dp.png");

        return result;
    }

    private void OnNewSongPlaying(SongModel newSong)
    {
        Title = newSong.Title;
        Artist = newSong.Artist;
        Album = newSong.Album;
        Duration = newSong.Duration;
        DurationTimeSliderValue = Duration.TotalSeconds;

        Cover = newSong.CoverImage;

        Songs.FirstOrDefault(song => song.Path == newSong.Path)!.IsPlaying = true;

        CurrentSongPath = MediaSource.FromFile(Songs[Songs.IndexOf(newSong)].Path);

        UpdateStringRepresentation();
    }

    private void UpdateStringRepresentation()
    {
        CurrentTimeStringRepresentation = TimeSpan.Zero.ToString(@"mm\:ss");

        DurationStringRepresentation = Duration.ToString(@"mm\:ss");
    }
}