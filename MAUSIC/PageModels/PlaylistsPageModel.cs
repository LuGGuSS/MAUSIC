using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Data.Constants;
using MAUSIC.Managers;
using MAUSIC.Mappers;
using MAUSIC.Models;
using MAUSIC.Models.Abstract;
using MAUSIC.PageModels.Abstract;

namespace MAUSIC.PageModels;

public partial class PlaylistsPageModel : BasePageModel
{
    private readonly PlaylistManager _playlistManager;
    private readonly SongsManager _songsManager;
    private readonly QueueManager _queueManager;

    private List<BaseModel> _playlists = new();

    public PlaylistsPageModel(
        PlaylistManager playlistManager,
        SongsManager songsManager,
        QueueManager queueManager)
    {
        _playlistManager = playlistManager;
        _songsManager = songsManager;
        _queueManager = queueManager;
    }

    [ObservableProperty] private List<BaseModel> _items = new();

    [ObservableProperty] private bool _shouldShowFooter;

    protected override async Task InitializeAsync()
    {
        var playlistsEntities = await _playlistManager.GetAllPlaylists();

        foreach (var entity in playlistsEntities)
        {
            var model = entity.Map();

            if (model != null)
            {
                _playlists.Add(model);
            }
        }

        Items = _playlists;
    }

    [RelayCommand]
    private async Task OnPlaylistSelected(PlaylistModel playlistModel)
    {
        /*if (playlistModel.Songs.Count == 0)
        {*/
            var playlistSongEntities = await _playlistManager.GetPlaylistSongs(playlistModel.Id);

            var playlistSongModels = new List<BaseModel>();

            foreach (var playlistSongEntity in playlistSongEntities)
            {
                var entity = await _songsManager.GetSongFromId(playlistSongEntity.SongId);

                var model = entity.Map();

                if (model != null)
                {
                    model.OpenPopupFunc = GetPopupTask;

                    playlistSongModels.Add(model);
                }
            }

            playlistModel.Songs = playlistSongModels;
        //}

        Items = playlistModel.Songs;

        ShouldShowFooter = true;
    }

    [RelayCommand]
    private async Task OnSongSelected(SongModel songModel)
    {
        var songs = new ObservableCollection<SongModel>();

        foreach (var item in Items)
        {
            if (item is SongModel song)
            {
                songs.Add(song);
            }
        }

        if (songs.Count == 0)
        {
            return;
        }

        var queue = _queueManager.GetCurrentSongsQueue?.Invoke();

        if (queue == null)
        {
            return;
        }

        queue.Songs = songs;
        queue.CurrentSongIndex = songs.IndexOf(songModel);

        await Shell.Current.GoToAsync($"///PlayerPage?StartPlaying={true}");
    }

    [RelayCommand]
    private void NavigateBack()
    {
        Items = _playlists;

        ShouldShowFooter = false;
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
                _queueManager.AddSongToQueue(queue, song.Map());
                break;
            case MoreMenuConstants.RemoveFromQueue:
                _queueManager.RemoveSongFromQueue(queue, songFromQueue.Map());
                break;
        }
    }
}