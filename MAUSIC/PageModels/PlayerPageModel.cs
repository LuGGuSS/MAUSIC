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
        await InitialLoadSongs();
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

        await LoadSongsFromFiles(files);
    }

    public async Task RequestFiles()
    {
        var files = await _storageManager.PickFolder();

        if (files == null || files.Count == 0)
        {
            return;
        }

        await LoadSongsFromFiles(files);
    }

    private async Task LoadSongsFromFiles(IList<string> files)
    {
        var songsEntities = new List<SongEntity>();

        var dbCheckTasks = new List<Task>();

        foreach (var file in files)
        {
            var existingSong = await _databaseManager.GetItemAsync<SongEntity>((song) => song.Path == file);

            if (existingSong != null)
            {
                continue;
            }

            var task = Task.Run(() =>
            {
                try
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

                    _semaphore.Wait();
                    songsEntities.Add(song);
                    _semaphore.Release();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });

            dbCheckTasks.Add(task);
        }

        await Task.WhenAll(dbCheckTasks);

        await _databaseManager.SaveItemsAsync(songsEntities);

        var allSongEntities = await _databaseManager.GetAllItems<SongEntity>();

        if (allSongEntities == null)
        {
            return;
        }

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

        Queue.Songs = songModels.OrderBy(songModel => songModel.Path).ToList();

        if (Queue.CurrentSong == null)
        {
            return;
        }

        OnNewSongPlaying();
    }

    [RelayCommand]
    private void EnqueueNextSong()
    {
        _queueManager.EnqueueNextSong(Queue);

        if (Queue.CurrentSong == null)
        {
            return;
        }

        OnNewSongPlaying();
    }

    [RelayCommand]
    private void EnqueuePreviousSong()
    {
        _queueManager.EnqueuePreviousSong(Queue);

        if (Queue.CurrentSong == null)
        {
            return;
        }

        OnNewSongPlaying();
    }

    [RelayCommand]
    private void QueueSongSelected(SongModel song)
    {
        if (Queue.Songs == null)
        {
            return;
        }

        var index = Queue.Songs.IndexOf(song);

        _queueManager.EnqueueSongByIndex(Queue, index);

        if (Queue.CurrentSong == null)
        {
            return;
        }

        OnNewSongPlaying();
    }

    private ImageSource TryGetSongCover(string songPath)
    {
        var tags = TagLib.File.Create(songPath);

        var result =tags.Tag.Pictures.Length > 0
            ? ImageSource.FromStream(() => new MemoryStream(tags.Tag.Pictures[0].Data.Data))
            : ImageSource.FromFile("album_100dp.png");

        return result;
    }

    private void OnNewSongPlaying()
    {
        if (Queue?.Songs == null || Queue.CurrentSong == null)
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