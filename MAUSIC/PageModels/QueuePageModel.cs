using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Data.Constants;
using MAUSIC.Managers;
using MAUSIC.Mappers;
using MAUSIC.Models;
using MAUSIC.PageModels.Abstract;

namespace MAUSIC.PageModels;

public partial class QueuePageModel : BasePageModel
{
    private readonly SongsManager _songsManager;
    private readonly PlaylistManager _playlistManager;
    private readonly QueueManager _queueManager;

    [ObservableProperty] private SongsQueue _queue;

    public QueuePageModel(
        SongsManager songsManager,
        PlaylistManager playlistManager,
        QueueManager queueManager)
    {
        _songsManager = songsManager;
        _playlistManager = playlistManager;
        _queueManager = queueManager;
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

                ShowPopupAsync.Invoke((o) => HandlePopup(o, model.Id));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        });
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

        switch (selectedOption)
        {
            case MoreMenuConstants.AddToPlaylist:
                var addToTestPlaylistEntity = await _playlistManager.GetPlaylistByTitle(PlaylistsConstants.TestPlaylist);
                if (addToTestPlaylistEntity != null)
                {
                    await _playlistManager.AddSong(addToTestPlaylistEntity, song);
                }
                break;
            case MoreMenuConstants.RemoveFromPlaylist:
                var removeFromTestPlaylistEntity = await _playlistManager.GetPlaylistByTitle(PlaylistsConstants.TestPlaylist);
                if (removeFromTestPlaylistEntity != null)
                {
                    await _playlistManager.RemoveSong(removeFromTestPlaylistEntity, song);
                }
                break;
            case MoreMenuConstants.Favourite:
                var favouritePlaylist = await _playlistManager.GetPlaylistByTitle(PlaylistsConstants.FavoriteSongs);
                if (favouritePlaylist != null)
                {
                    var newFavouriteState = await _playlistManager.ToggleFavourite(favouritePlaylist, song);
                    song.IsFavorite = newFavouriteState;

                    await _songsManager.SaveSong(song);

                    var existingQueueSong = queue?.Songs?.FirstOrDefault((queuedSong) => song.Id == queuedSong.Id);

                    if (existingQueueSong != null)
                    {
                        existingQueueSong.IsFavorite = newFavouriteState;
                    }
                }
                break;
            case MoreMenuConstants.AddToQueue:
                break;
            case MoreMenuConstants.RemoveFromQueue:
                _queueManager.RemoveSongFromQueue(queue, songFromQueue.Map());
                break;
        }
    }
}