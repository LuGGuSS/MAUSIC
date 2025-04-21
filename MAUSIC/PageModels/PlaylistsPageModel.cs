using System.Collections.ObjectModel;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MAUSIC.Data.Constants;
using MAUSIC.Data.Entities;
using MAUSIC.Managers;
using MAUSIC.Mappers;
using MAUSIC.Models;
using MAUSIC.Models.Abstract;
using MAUSIC.PageModels.Abstract;
using MAUSIC.Views;

namespace MAUSIC.PageModels;

public partial class PlaylistsPageModel : BasePageModel
{
    private readonly PlaylistManager _playlistManager;
    private readonly SongsManager _songsManager;
    private readonly QueueManager _queueManager;

    private ObservableCollection<BaseModel> _playlists = new();

    public PlaylistsPageModel(
        PlaylistManager playlistManager,
        SongsManager songsManager,
        QueueManager queueManager)
    {
        _playlistManager = playlistManager;
        _songsManager = songsManager;
        _queueManager = queueManager;
    }

    [ObservableProperty] private ObservableCollection<BaseModel> _items = new();

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

        Items = playlistModel.Songs.ToObservableCollection();

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

    [RelayCommand]
    private async Task CreatePlaylist(string playlistName)
    {
        var result = await _playlistManager.CreatePlaylist(playlistName);

        var model = result.Map();

        if (model == null)
        {
            return;
        }

        if (ShouldShowFooter)
        {
            _playlists.Add(model);
        }
        else
        {
            Items.Add(model);
        }
    }

    private Task GetPopupTask(SongModel model)
    {
        return Task.Run(() =>
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