using System.Collections;
using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Data.Constants;
using MAUSIC.Data.Entities;
using MAUSIC.Managers;
using MAUSIC.Mappers;
using MAUSIC.Models;
using BasePageModel = MAUSIC.PageModels.Abstract.BasePageModel;

namespace MAUSIC.PageModels;

public partial class PlayerPageModel : BasePageModel
{
    private readonly QueueManager _queueManager;
    private readonly RecommendationManager _recommendationManager;
    private readonly PlaylistManager _playlistManager;

    private readonly SemaphoreSlim _semaphore = new(1);

    private CancellationTokenSource _cancellationTokenSource;

    [ObservableProperty] private string _title;

    [ObservableProperty] private string _artist;

    [ObservableProperty] private string _album;

    [ObservableProperty] private bool _isOnRepeat;

    [ObservableProperty] private bool _isShuffled;

    [ObservableProperty] private ImageSource _cover = ImageSource.FromFile("album_100dp.png");

    [ObservableProperty] private TimeSpan _duration = TimeSpan.Zero;

    [ObservableProperty] private double _durationTimeSliderValue;

    [ObservableProperty] private string _durationStringRepresentation;

    [ObservableProperty] private double _currentTimeSliderValue;

    [ObservableProperty] private string _currentTimeStringRepresentation;

    [ObservableProperty] private bool _isFavourite;

    [ObservableProperty] private bool _isRecommended;

    [ObservableProperty] private MediaSource? _currentSongPath;

    [ObservableProperty] private SongsQueue _queue = new SongsQueue();

    public PlayerPageModel(
        QueueManager queueManager,
        RecommendationManager recommendationManager,
        PlaylistManager playlistManager)
    {
        _queueManager = queueManager;
        _recommendationManager = recommendationManager;
        _playlistManager = playlistManager;

        _queueManager.GetCurrentSongsQueue = () => Queue;
        Queue.OnNewSongPlaying = OnNewSongPlaying;

        _cancellationTokenSource = new CancellationTokenSource();
    }

    protected override async Task InitializeAsync()
    {
        Title = "Song";
        Artist = "Artist";
        Album = "Album";
        UpdateStringRepresentation();
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

    [RelayCommand]
    private async Task ToggleFavourite()
    {
        if (Queue.CurrentSong == null)
        {
            return;
        }

        var result = await _playlistManager.ToggleFavourite(Queue.CurrentSong.Map()!);
        Queue.CurrentSong.IsFavourite = result;
        IsFavourite = result;
    }

    [RelayCommand]
    private async Task ChangeRecommendationWeight(float weightChange)
    {
        var queue = Queue.Songs.Select((model) => model.Map()).ToList();
        queue.RemoveAt(Queue.CurrentSongIndex);
        await _recommendationManager.ChangeRecommendationWeight(queue!, Queue.CurrentSong.Map()!, weightChange);
    }

    private async void OnNewSongPlaying()
    {
        if (Queue.Songs == null || Queue.CurrentSong == null)
        {
            return;
        }

        Title = Queue.CurrentSong.Title;
        Artist = Queue.CurrentSong.Artist;
        Album = Queue.CurrentSong.Album;
        Duration = Queue.CurrentSong.Duration;
        IsFavourite = Queue.CurrentSong.IsFavourite;
        IsRecommended = Queue.CurrentSong.IsRecommended;
        DurationTimeSliderValue = Duration.TotalSeconds;

        Cover = Queue.CurrentSong.CoverImage;

        CurrentSongPath = MediaSource.FromFile(Queue.CurrentSong.Path);

        UpdateStringRepresentation();

        await GetRecommendations();
    }

    private void UpdateStringRepresentation()
    {
        CurrentTimeStringRepresentation = TimeSpan.Zero.ToString(@"mm\:ss");

        DurationStringRepresentation = Duration.ToString(@"mm\:ss");
    }

    private async Task GetRecommendations()
    {
        if (Queue.Songs.Count > 0
            && Queue.Songs.Count - Queue.CurrentSongIndex <= RecommendationConstants.RecommendationThreshold
            && !IsOnRepeat)
        {
            var recommendations = await _recommendationManager.GetRecommendation(
                new List<SongEntity>(Queue.Songs
                    .Select(song =>
                        song.Map())
                    .ToList()!),
                RecommendationConstants.RecommendationCount);

            recommendations.ForEach(recommendation =>
            {
                var model = recommendation.Map();
                if (model == null)
                {
                    return;
                }

                model.IsRecommended = true;
                Queue.AddSong(model);
            });
        }
    }

    async partial void OnIsOnRepeatChanged(bool value)
    {
        await GetRecommendations();
    }

    partial void OnIsShuffledChanged(bool value)
    {
        Queue.ToggleShuffle(value);
    }
}