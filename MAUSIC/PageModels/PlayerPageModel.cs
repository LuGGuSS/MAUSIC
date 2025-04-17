using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Data.Entities;
using MAUSIC.Managers;
using MAUSIC.Mappers;
using MAUSIC.Models;
using BasePageModel = MAUSIC.PageModels.Abstract.BasePageModel;

namespace MAUSIC.PageModels;

public partial class PlayerPageModel : BasePageModel
{
    private readonly StorageManager _storageManager;
    private readonly SongsManager _songsManager;
    private readonly DatabaseManager _databaseManager;
    private readonly QueueManager _queueManager;

    private readonly SemaphoreSlim _semaphore = new(1);

    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty] private string _title;

    [ObservableProperty] private string _artist;

    [ObservableProperty] private string _album;

    [ObservableProperty] private ImageSource _cover = ImageSource.FromFile("album_100dp.png");

    [ObservableProperty] private TimeSpan _duration = TimeSpan.Zero;

    [ObservableProperty] private double _durationTimeSliderValue;

    [ObservableProperty] private string _durationStringRepresentation;

    [ObservableProperty] private double _currentTimeSliderValue;

    [ObservableProperty] private string _currentTimeStringRepresentation;

    [ObservableProperty] private MediaSource? _currentSongPath;

    [ObservableProperty] private SongsQueue _queue = new SongsQueue();

    public PlayerPageModel(
        StorageManager storageManager,
        SongsManager songsManager,
        DatabaseManager databaseManager,
        QueueManager queueManager)
    {
        _storageManager = storageManager;
        _songsManager = songsManager;
        _databaseManager = databaseManager;
        _queueManager = queueManager;

        _queueManager.GetCurrentSongsQueue = () => Queue;
        Queue.OnNewSongPlaying = OnNewSongPlaying;

        _cancellationTokenSource = new CancellationTokenSource();
    }

    protected override async Task InitializeAsync()
    {
        // await InitialLoadSongs();
        Title = "Song";
        Artist = "Artist";
        Album = "Album";
        UpdateStringRepresentation();
    }

    private async Task InitialLoadSongs()
    {
        var folders = await _databaseManager.GetAllItems<FolderEntity>();

        if (folders == null || folders.Count == 0)
        {
            Title = "Song";
            Artist = "Artist";
            Album = "Album";
            UpdateStringRepresentation();

            return;
        }

        List<string> files = new List<string>();

        foreach (var folder in folders)
        {
            var resultFiles = await _storageManager.GetMusicFiles(folder.Path);
            files.AddRange(resultFiles);
        }

        await _songsManager.LoadSongsFromFilesAsync(files);
    }

    [RelayCommand]
    private void EnqueueNextSong()
    {
        _queueManager.EnqueueNextSong(Queue);
    }

    [RelayCommand]
    private void EnqueuePreviousSong()
    {
        _queueManager.EnqueuePreviousSong(Queue);
    }

    private void OnNewSongPlaying()
    {
        if (Queue.Songs == null || Queue.CurrentSong == null)
        {
            return;
        }

        Title = Queue.CurrentSong.Title;
        Artist = Queue.CurrentSong.Artist;
        Album = Queue.CurrentSong.Album;
        Duration = Queue.CurrentSong.Duration;
        DurationTimeSliderValue = Duration.TotalSeconds;

        Cover = Queue.CurrentSong.CoverImage;

        CurrentSongPath = MediaSource.FromFile(Queue.CurrentSong.Path);

        UpdateStringRepresentation();
    }

    private void UpdateStringRepresentation()
    {
        CurrentTimeStringRepresentation = TimeSpan.Zero.ToString(@"mm\:ss");

        DurationStringRepresentation = Duration.ToString(@"mm\:ss");
    }
}