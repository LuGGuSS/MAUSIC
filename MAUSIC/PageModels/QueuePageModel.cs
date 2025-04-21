using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Data.Constants;
using MAUSIC.Data.Entities;
using MAUSIC.Managers;
using MAUSIC.Mappers;
using MAUSIC.Models;
using MAUSIC.PageModels.Abstract;
using MAUSIC.Views;

namespace MAUSIC.PageModels;

public partial class QueuePageModel : BasePageModel
{
    private readonly SongsManager _songsManager;
    private readonly PlaylistManager _playlistManager;
    private readonly QueueManager _queueManager;
    private readonly RecommendationManager _recommendationManager;

    [ObservableProperty] private SongsQueue _queue;

    public QueuePageModel(
        SongsManager songsManager,
        PlaylistManager playlistManager,
        QueueManager queueManager,
        RecommendationManager recommendationManager)
    {
        _songsManager = songsManager;
        _playlistManager = playlistManager;
        _queueManager = queueManager;
        _recommendationManager = recommendationManager;
    }

    protected override Task InitializeAsync()
    {
        Queue = _queueManager.GetCurrentSongsQueue?.Invoke() ?? new SongsQueue();

        Queue.OpenPopupFunc = GetPopupTask;

        return Task.CompletedTask;
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
    }

    private Task GetPopupTask(SongModel model)
    {
        return Task.Run(async void () =>
        {
            try
            {
                if (ShowPopupAsync == null)
                {
                    return;
                }

                var models = new List<MoreOptionModel>
                {
                    new() { Title = MoreMenuConstants.AddToPlaylist, ResultItem = MoreMenuConstants.AddToPlaylist},
                    new() { Title = MoreMenuConstants.RemoveFromPlaylist, ResultItem = MoreMenuConstants.RemoveFromPlaylist},
                    new() { Title = MoreMenuConstants.Favourite,  ResultItem = MoreMenuConstants.Favourite},
                    new() { Title = MoreMenuConstants.AddToQueue, ResultItem = MoreMenuConstants.AddToQueue},
                    new() { Title = MoreMenuConstants.RemoveFromQueue, ResultItem = MoreMenuConstants.RemoveFromQueue},
                };

                ShowPopupAsync.Invoke(new MoreView( models, (o) => HandlePopup(o, model.Id)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
    }

    private async void AddToPlaylistCallback(object? playlistId, SongEntity song)
    {
        if (playlistId is not int id)
        {
            return;
        }

        var addToTestPlaylistEntity = await _playlistManager.GetPlaylistById(id);
        if (addToTestPlaylistEntity != null)
        {
            await _playlistManager.AddSong(addToTestPlaylistEntity, song);
        }
    }

    private async void RemoveFromPlaylistCallback(object? playlistId, SongEntity song)
    {
        if (playlistId is not int id)
        {
            return;
        }

        var addToTestPlaylistEntity = await _playlistManager.GetPlaylistById(id);
        if (addToTestPlaylistEntity != null)
        {
            await _playlistManager.RemoveSong(addToTestPlaylistEntity, song);
        }
    }

    private async void HandlePopup(object? result, int songId)
    {
        if (result is not string selectedOption)
        {
            return;
        }

        var queue = _queueManager.GetCurrentSongsQueue?.Invoke();

        var song = await _songsManager.GetSongFromId(songId);

        var songFromQueue = queue?.Songs?.FirstOrDefault(model => model.Id == songId).Map();

        if (song is null)
        {
            return;
        }

        var allPlaylists = await _playlistManager.GetAllPlaylists();
        allPlaylists.RemoveRange(0, 2);

        var playlistOptionModels = allPlaylists
            .Select((playlist) =>
                new MoreOptionModel()
                {
                    Title = playlist.Title,
                    ResultItem = playlist.Id
                }).ToList();

        switch (selectedOption)
        {
            case MoreMenuConstants.AddToPlaylist:
                ShowPopupAsync?.Invoke(new MoreView(
                    playlistOptionModels,
                    (playlistId) => AddToPlaylistCallback(playlistId, song)));

                break;
            case MoreMenuConstants.RemoveFromPlaylist:
                ShowPopupAsync?.Invoke(new MoreView(
                    playlistOptionModels,
                    (playlistId) => RemoveFromPlaylistCallback(playlistId, song)));
                break;
            case MoreMenuConstants.Favourite:
                var newFavouriteState = await _playlistManager.ToggleFavourite(song);
                song.IsFavourite = newFavouriteState;

                await _songsManager.SaveSong(song);

                var existingQueueSong = queue?.Songs?.FirstOrDefault((queuedSong) => song.Id == queuedSong.Id);

                if (existingQueueSong != null)
                {
                    existingQueueSong.IsFavourite = newFavouriteState;
                }
                break;
            case MoreMenuConstants.AddToQueue:
                _queueManager.AddSongToQueue(queue, song.Map());
                break;
            case MoreMenuConstants.RemoveFromQueue:
                _queueManager.RemoveSongFromQueue(queue, songFromQueue.Map());
                break;
        }
    }
}